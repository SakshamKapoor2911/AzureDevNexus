const passport = require('passport');
const BearerStrategy = require('passport-azure-ad').BearerStrategy;
const logger = require('../utils/logger');

/**
 * Configure Passport.js with Azure AD Bearer strategy
 */
const configurePassport = () => {
  // Azure AD Bearer Strategy for API authentication
  const bearerStrategy = new BearerStrategy({
    identityMetadata: `https://login.microsoftonline.com/${process.env.AZURE_AD_TENANT_ID || 'common'}/v2.0/.well-known/openid_configuration`,
    clientID: process.env.AZURE_AD_CLIENT_ID,
    validateIssuer: true,
    issuer: `https://login.microsoftonline.com/${process.env.AZURE_AD_TENANT_ID || 'common'}/v2.0`,
    passReqToCallback: false,
    loggingLevel: process.env.NODE_ENV === 'development' ? 'info' : 'warn'
  }, (token, done) => {
    try {
      // Verify the token and extract user information
      const user = {
        id: token.sub || token.oid,
        displayName: token.name || token.preferred_username,
        email: token.email || token.upn,
        roles: token.roles || [],
        groups: token.groups || [],
        azureAdId: token.sub || token.oid,
        tenantId: token.tid,
        aud: token.aud,
        iss: token.iss,
        exp: token.exp,
        iat: token.iat
      };

      logger.info({
        message: 'Azure AD token validated successfully',
        user: user.id,
        email: user.email,
        tenantId: user.tenantId
      });

      return done(null, user);
    } catch (error) {
      logger.error({
        message: 'Azure AD token validation failed',
        error: error.message,
        token: token ? 'present' : 'missing'
      });

      return done(error, null);
    }
  });

  // Use the bearer strategy
  passport.use(bearerStrategy);

  // Serialize user for the session
  passport.serializeUser((user, done) => {
    done(null, user);
  });

  // Deserialize user from the session
  passport.deserializeUser((user, done) => {
    done(null, user);
  });

  logger.info('Passport.js configured with Azure AD Bearer strategy');
};

/**
 * Initialize Passport.js middleware
 * @param {Object} app - Express app instance
 */
const initializePassport = (app) => {
  app.use(passport.initialize());
  app.use(passport.session());
  
  logger.info('Passport.js middleware initialized');
};

module.exports = {
  configurePassport,
  initializePassport
};
