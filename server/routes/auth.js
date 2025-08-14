const express = require('express');
const passport = require('passport');
const jwt = require('jsonwebtoken');
const { body, validationResult } = require('express-validator');
const logger = require('../utils/logger');
const { authRateLimit, verifyToken } = require('../middleware/auth');

const router = express.Router();

/**
 * @route   POST /api/auth/login
 * @desc    Authenticate user with Azure AD
 * @access  Public
 */
router.post('/login', [
  authRateLimit,
  body('username').notEmpty().withMessage('Username is required'),
  body('password').notEmpty().withMessage('Password is required')
], async (req, res) => {
  try {
    // Check for validation errors
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({
        success: false,
        error: {
          message: 'Validation failed',
          details: errors.array()
        }
      });
    }

    const { username, password } = req.body;

    // In a real implementation, you would validate against Azure AD here
    // For now, we'll simulate the authentication process
    logger.info({
      message: 'Login attempt',
      username: username,
      ip: req.ip
    });

    // Simulate Azure AD authentication
    // In production, this would use passport-azure-ad or similar
    if (username === 'admin' && password === 'admin') {
      // Generate JWT token
      const token = jwt.sign(
        {
          id: '1',
          username: username,
          email: `${username}@azuredevnexus.com`,
          role: 'admin',
          azureAdId: 'azure-ad-user-id'
        },
        process.env.JWT_SECRET || 'fallback-secret',
        { expiresIn: process.env.JWT_EXPIRES_IN || '24h' }
      );

      // Set token in cookie
      res.cookie('token', token, {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
        maxAge: 24 * 60 * 60 * 1000 // 24 hours
      });

      logger.info({
        message: 'Login successful',
        username: username,
        ip: req.ip
      });

      return res.status(200).json({
        success: true,
        message: 'Authentication successful',
        data: {
          user: {
            id: '1',
            username: username,
            email: `${username}@azuredevnexus.com`,
            role: 'admin'
          },
          token: token
        }
      });
    }

    // Authentication failed
    logger.warn({
      message: 'Login failed - invalid credentials',
      username: username,
      ip: req.ip
    });

    return res.status(401).json({
      success: false,
      error: {
        message: 'Invalid credentials',
        code: 'INVALID_CREDENTIALS'
      }
    });

  } catch (error) {
    logger.error({
      message: 'Login error',
      error: error.message,
      stack: error.stack,
      ip: req.ip
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Internal server error',
        code: 'INTERNAL_ERROR'
      }
    });
  }
});

/**
 * @route   POST /api/auth/azure/callback
 * @desc    Azure AD OAuth callback
 * @access  Public
 */
router.get('/azure/callback', 
  passport.authenticate('azure-ad-bearer', { session: false }),
  async (req, res) => {
    try {
      if (!req.user) {
        return res.status(401).json({
          success: false,
          error: {
            message: 'Azure AD authentication failed',
            code: 'AZURE_AUTH_FAILED'
          }
        });
      }

      // Generate JWT token for Azure AD user
      const token = jwt.sign(
        {
          id: req.user.id || req.user.sub,
          username: req.user.name || req.user.preferred_username,
          email: req.user.email || req.user.upn,
          role: req.user.role || 'user',
          azureAdId: req.user.sub,
          azureAdGroups: req.user.groups || []
        },
        process.env.JWT_SECRET || 'fallback-secret',
        { expiresIn: process.env.JWT_EXPIRES_IN || '24h' }
      );

      // Set token in cookie
      res.cookie('token', token, {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'strict',
        maxAge: 24 * 60 * 60 * 1000 // 24 hours
      });

      logger.info({
        message: 'Azure AD authentication successful',
        user: req.user.sub,
        ip: req.ip
      });

      // Redirect to frontend with token
      res.redirect(`${process.env.CORS_ORIGIN || 'http://localhost:3000'}/auth/success?token=${token}`);

    } catch (error) {
      logger.error({
        message: 'Azure AD callback error',
        error: error.message,
        stack: error.stack,
        ip: req.ip
      });

      res.redirect(`${process.env.CORS_ORIGIN || 'http://localhost:3000'}/auth/error`);
    }
  }
);

/**
 * @route   POST /api/auth/refresh
 * @desc    Refresh JWT token
 * @access  Private
 */
router.post('/refresh', verifyToken, async (req, res) => {
  try {
    const user = req.user;

    // Generate new token
    const newToken = jwt.sign(
      {
        id: user.id,
        username: user.username,
        email: user.email,
        role: user.role,
        azureAdId: user.azureAdId
      },
      process.env.JWT_SECRET || 'fallback-secret',
      { expiresIn: process.env.JWT_EXPIRES_IN || '24h' }
    );

    // Update cookie
    res.cookie('token', newToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === 'production',
      sameSite: 'strict',
      maxAge: 24 * 60 * 60 * 1000 // 24 hours
    });

    logger.info({
      message: 'Token refreshed',
      user: user.id,
      ip: req.ip
    });

    return res.status(200).json({
      success: true,
      message: 'Token refreshed successfully',
      data: {
        token: newToken
      }
    });

  } catch (error) {
    logger.error({
      message: 'Token refresh error',
      error: error.message,
      stack: error.stack,
      ip: req.ip
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Internal server error',
        code: 'INTERNAL_ERROR'
      }
    });
  }
});

/**
 * @route   POST /api/auth/logout
 * @desc    Logout user and clear token
 * @access  Private
 */
router.post('/logout', verifyToken, async (req, res) => {
  try {
    // Clear token cookie
    res.clearCookie('token');

    logger.info({
      message: 'User logged out',
      user: req.user.id,
      ip: req.ip
    });

    return res.status(200).json({
      success: true,
      message: 'Logout successful'
    });

  } catch (error) {
    logger.error({
      message: 'Logout error',
      error: error.message,
      stack: error.stack,
      ip: req.ip
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Internal server error',
        code: 'INTERNAL_ERROR'
      }
    });
  }
});

/**
 * @route   GET /api/auth/me
 * @desc    Get current user information
 * @access  Private
 */
router.get('/me', verifyToken, async (req, res) => {
  try {
    return res.status(200).json({
      success: true,
      data: {
        user: {
          id: req.user.id,
          username: req.user.username,
          email: req.user.email,
          role: req.user.role,
          azureAdId: req.user.azureAdId
        }
      }
    });

  } catch (error) {
    logger.error({
      message: 'Get user info error',
      error: error.message,
      stack: error.stack,
      ip: req.ip
    });

    return res.status(500).json({
      success: false,
      error: {
        message: 'Internal server error',
        code: 'INTERNAL_ERROR'
      }
    });
  }
});

/**
 * @route   POST /api/auth/validate
 * @desc    Validate JWT token
 * @access  Public
 */
router.post('/validate', async (req, res) => {
  try {
    const { token } = req.body;

    if (!token) {
      return res.status(400).json({
        success: false,
        error: {
          message: 'Token is required',
          code: 'TOKEN_MISSING'
        }
      });
    }

    const decoded = jwt.verify(token, process.env.JWT_SECRET || 'fallback-secret');
    
    return res.status(200).json({
      success: true,
      data: {
        valid: true,
        user: {
          id: decoded.id,
          username: decoded.username,
          email: decoded.email,
          role: decoded.role
        }
      }
    });

  } catch (error) {
    return res.status(200).json({
      success: true,
      data: {
        valid: false,
        error: error.message
      }
    });
  }
});

module.exports = router;
