# External Authentication Setup Guide

This guide will help you set up external authentication providers (Google and Microsoft) for the User module.

## Google Authentication

1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Navigate to "APIs & Services" > "Credentials"
4. Click "Create Credentials" > "OAuth client ID"
5. Select "Web application" as the application type
6. Add the following authorized redirect URIs:
   - `https://your-domain.com/api/auth/external-login-callback`
   - `http://localhost:5000/api/auth/external-login-callback` (for local development)
7. Click "Create"
8. Copy the Client ID and Client Secret
9. Update the `appsettings.json` with your credentials:
   ```json
   "Authentication": {
     "Google": {
       "ClientId": "your-client-id.apps.googleusercontent.com",
       "ClientSecret": "your-client-secret"
     }
   }
   ```

## Microsoft Authentication

1. Go to the [Azure Portal](https://portal.azure.com/)
2. Navigate to "Azure Active Directory" > "App registrations"
3. Click "New registration"
4. Enter a name for your application
5. Select "Web" as the platform
6. Add the following redirect URIs:
   - `https://your-domain.com/api/auth/external-login-callback`
   - `http://localhost:5000/api/auth/external-login-callback` (for local development)
7. Click "Register"
8. Copy the Application (client) ID
9. Go to "Certificates & secrets"
10. Create a new client secret
11. Copy the client secret value
12. Update the `appsettings.json` with your credentials:
    ```json
    "Authentication": {
      "Microsoft": {
        "ClientId": "your-client-id",
        "ClientSecret": "your-client-secret"
      }
    }
    ```

## Email Service Setup (Gmail)

1. Go to your [Google Account](https://myaccount.google.com/)
2. Navigate to "Security"
3. Enable "2-Step Verification" if not already enabled
4. Go to "App passwords"
5. Select "Mail" and "Other (Custom name)"
6. Enter "Modulith" as the name
7. Copy the generated 16-character password
8. Update the `appsettings.json` with your email settings:
   ```json
   "EmailSettings": {
     "FromEmail": "your-email@gmail.com",
     "FromName": "Modulith",
     "SmtpServer": "smtp.gmail.com",
     "SmtpPort": 587,
     "SmtpUsername": "your-email@gmail.com",
     "SmtpPassword": "your-16-character-app-password",
     "UseSsl": true
   }
   ```

## Security Notes

1. Never commit sensitive credentials to source control
2. Use environment variables or a secure configuration management system in production
3. Regularly rotate your secrets and passwords
4. Use HTTPS in production
5. Keep your dependencies up to date

## Testing External Authentication

1. Start the application
2. Navigate to `/api/auth/external-login?provider=Google` or `/api/auth/external-login?provider=Microsoft`
3. You should be redirected to the provider's login page
4. After successful login, you'll be redirected back to your application
5. Check the logs for any authentication errors 