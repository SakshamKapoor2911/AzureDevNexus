const jwt = require('jsonwebtoken');
const passport = require('passport');
const logger = require('../utils/logger');

/**
 * Verify JWT token middleware
 * @param {Object} req - Express request object
 * @param {Object} res - Express response object
 * @param {Function} next - Express next function
 */
const verifyToken = (req, res, next) => {
  try {
    const token = req.headers.authorization?.split(' ')[1] || 
                  req.cookies?.token || 
                  req.query?.token;

    if (!token) {
      return res.status(401).json({
        success: false,
        error: {
          message: 'Access token required',
          code: 'TOKEN_MISSING'
        }
      });
    }

    const decoded = jwt.verify(token, process.env.JWT_SECRET);
    req.user = decoded;
    next();
  } catch (error) {
    logger.warn({
      message: 'Token verification failed',
      error: error.message,
      ip: req.ip,
      userAgent: req.get('User-Agent')
    });

    if (error.name === 'TokenExpiredError') {
      return res.status(401).json({
        success: false,
        error: {
          message: 'Token expired',
          code: 'TOKEN_EXPIRED'
        }
      });
    }

    return res.status(401).json({
      success: false,
      error: {
        message: 'Invalid token',
        code: 'TOKEN_INVALID'
      }
    });
  }
};

/**
 * Azure AD authentication middleware
 * @param {Object} req - Express request object
 * @param {Object} res - Express response object
 * @param {Function} next - Express next function
 */
const azureAuth = (req, res, next) => {
  passport.authenticate('azure-ad-bearer', { session: false }, (err, user, info) => {
    if (err) {
      logger.error({
        message: 'Azure AD authentication error',
        error: err.message,
        ip: req.ip
      });
      return res.status(500).json({
        success: false,
        error: {
          message: 'Authentication service error',
          code: 'AUTH_SERVICE_ERROR'
        }
      });
    }

    if (!user) {
      return res.status(401).json({
        success: false,
        error: {
          message: 'Azure AD authentication failed',
          code: 'AZURE_AUTH_FAILED'
        }
      });
    }

    req.user = user;
    next();
  })(req, res, next);
};

/**
 * Role-based access control middleware
 * @param {Array} allowedRoles - Array of allowed roles
 * @returns {Function} Middleware function
 */
const requireRole = (allowedRoles) => {
  return (req, res, next) => {
    if (!req.user) {
      return res.status(401).json({
        success: false,
        error: {
          message: 'Authentication required',
          code: 'AUTH_REQUIRED'
        }
      });
    }

    const userRole = req.user.role || req.user.roles?.[0];
    
    if (!userRole || !allowedRoles.includes(userRole)) {
      logger.warn({
        message: 'Access denied - insufficient permissions',
        user: req.user.id || req.user.sub,
        requiredRoles: allowedRoles,
        userRole: userRole,
        ip: req.ip,
        path: req.path
      });

      return res.status(403).json({
        success: false,
        error: {
          message: 'Insufficient permissions',
          code: 'INSUFFICIENT_PERMISSIONS'
        }
      });
    }

    next();
  };
};

/**
 * Optional authentication middleware (doesn't fail if no token)
 * @param {Object} req - Express request object
 * @param {Object} res - Express response object
 * @param {Function} next - Express next function
 */
const optionalAuth = (req, res, next) => {
  try {
    const token = req.headers.authorization?.split(' ')[1] || 
                  req.cookies?.token || 
                  req.query?.token;

    if (token) {
      const decoded = jwt.verify(token, process.env.JWT_SECRET);
      req.user = decoded;
    }
    next();
  } catch (error) {
    // Continue without authentication
    next();
  }
};

/**
 * Rate limiting for authentication endpoints
 * @param {Object} req - Express request object
 * @param {Object} res - Express response object
 * @param {Function} next - Express next function
 */
const authRateLimit = (req, res, next) => {
  // This would typically be implemented with a more sophisticated rate limiting library
  // For now, we'll use a simple approach
  const clientIP = req.ip;
  const now = Date.now();
  
  // In a real implementation, you'd store this in Redis or a database
  // This is a simplified version
  if (!req.app.locals.authAttempts) {
    req.app.locals.authAttempts = new Map();
  }

  const attempts = req.app.locals.authAttempts.get(clientIP) || { count: 0, resetTime: now + 15 * 60 * 1000 };
  
  if (now > attempts.resetTime) {
    attempts.count = 0;
    attempts.resetTime = now + 15 * 60 * 1000;
  }

  if (attempts.count >= 5) {
    logger.warn({
      message: 'Too many authentication attempts',
      ip: clientIP,
      userAgent: req.get('User-Agent')
    });

    return res.status(429).json({
      success: false,
      error: {
        message: 'Too many authentication attempts, please try again later',
        code: 'TOO_MANY_ATTEMPTS'
      }
    });
  }

  attempts.count++;
  req.app.locals.authAttempts.set(clientIP, attempts);
  next();
};

module.exports = {
  verifyToken,
  azureAuth,
  requireRole,
  optionalAuth,
  authRateLimit
};
