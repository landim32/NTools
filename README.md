# NTools API

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=landim32_NTools&metric=alert_status)](https://sonarcloud.io/project/overview?id=landim32_NTools)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=landim32_NTools&metric=coverage)](https://sonarcloud.io/project/overview?id=landim32_NTools)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=landim32_NTools&metric=bugs)](https://sonarcloud.io/project/overview?id=landim32_NTools)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=landim32_NTools&metric=code_smells)](https://sonarcloud.io/project/overview?id=landim32_NTools)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=landim32_NTools&metric=vulnerabilities)](https://sonarcloud.io/project/overview?id=landim32_NTools)

## Overview

NTools API is a comprehensive RESTful API service built with .NET 8 that provides a collection of utility tools and services for common development tasks. The API offers functionalities including document validation (CPF/CNPJ), email services, file management with S3-compatible storage, and string manipulation utilities.

The project follows a clean architecture approach with separated layers for API, Application, Domain, ACL (Anti-Corruption Layer), DTOs, and comprehensive test coverage.

## Environment Configuration

Before running the application, you need to configure the environment variables:

1. Copy the `.env.example` file to `.env`:
    ```bash
    cp .env.example .env
    ```

2. Edit the `.env` file and fill in your actual credentials:
    ```bash
    # NTools API Configuration
    NTOOLS_API_URL=http://*:80

    # MailerSend Configuration
    MAILERSEND__MAILSENDER=your-email@example.com
    MAILERSEND__APIURL=https://api.mailersend.com/v1/email
    MAILERSEND__APITOKEN=your-mailersend-api-token

    # DigitalOcean Spaces (S3) Configuration
    S3__ACCESSKEY=your-digitalocean-spaces-access-key
    S3__SECRETKEY=your-digitalocean-spaces-secret-key
    S3__ENDPOINT=https://your-space-name.nyc3.digitaloceanspaces.com
    ```

    ⚠️ **IMPORTANT**: Never commit the `.env` file with real credentials. Only the `.env.example` should be version controlled.

## Docker Setup

### Running NTools API with Docker

#### Build the Docker image
```bash
docker compose build
```

#### Start the container
```bash
docker compose up -d
```

#### View container logs
```bash
docker compose logs -f ntools-api
```

#### Stop the container
```bash
docker compose down
```

#### Complete rebuild (clear cache)
```bash
docker compose build --no-cache
docker compose up -d
```

### Accessing the API
After starting the container, the API will be available at:
- **API Base URL**: http://localhost:5001
- **Swagger UI**: http://localhost:5001/swagger

## Technologies Used

### Core Framework
- **.NET 8.0** - Modern, cross-platform framework for building web APIs
- **ASP.NET Core** - Web framework for building HTTP services

### Cloud & Storage
- **AWS SDK for S3 (4.0.6.10)** - S3-compatible storage (DigitalOcean Spaces)
- **MailerSend API** - Email delivery service integration

### Image Processing
- **SixLabors.ImageSharp (3.1.11)** - Modern image processing library
- **System.Drawing.Common (9.0.8)** - Graphics library support

### Additional Libraries
- **Newtonsoft.Json (13.0.3)** - JSON serialization/deserialization
- **Stripe.NET (48.5.0)** - Payment processing integration
- **Swashbuckle.AspNetCore (9.0.4)** - Swagger/OpenAPI documentation
- **Entity Framework Core Proxies (9.0.8)** - ORM with lazy loading support

### Testing
- **xUnit** - Unit testing framework
- Comprehensive test coverage across all layers

### DevOps
- **Docker** - Containerization
- **GitHub Actions** - CI/CD with SonarCloud integration
- **Azure Container Instances** - Cloud deployment

## API Documentation

The NTools API provides four main controllers with various endpoints for different utility functions.

### Document Controller

Validates Brazilian tax identification documents (CPF and CNPJ).

#### Endpoints

**GET** `/Document/validarCpfOuCnpj/{cpfCnpj}`

Validates if a CPF or CNPJ number is valid.

- **Parameters**:
  - `cpfCnpj` (string, path): The CPF or CNPJ number to validate
- **Returns**: 
  - `200 OK` - Boolean indicating if the document is valid
  - `500 Internal Server Error` - Error message if validation fails
- **Example**:
  ```
  GET /Document/validarCpfOuCnpj/12345678900
  ```

### Mail Controller

Handles email operations including sending emails and validating email addresses.

#### Endpoints

**POST** `/Mail/sendMail`

Sends an email using the MailerSend service.

- **Request Body**: `MailerInfo` object
  - `to` (string): Recipient email address
  - `subject` (string): Email subject
  - `from` (string): Sender email address
  - `html` (string): HTML content of the email
- **Returns**:
  - `200 OK` - Boolean indicating if the email was sent successfully
  - `500 Internal Server Error` - Error message if sending fails
- **Example**:
  ```json
  POST /Mail/sendMail
  {
    "to": "recipient@example.com",
    "subject": "Test Email",
    "from": "sender@example.com",
    "html": "<h1>Hello World</h1>"
  }
  ```

**GET** `/Mail/isValidEmail/{email}`

Validates if an email address has a valid format.

- **Parameters**:
  - `email` (string, path): The email address to validate
- **Returns**:
  - `200 OK` - Boolean indicating if the email format is valid
  - `500 Internal Server Error` - Error message if validation fails
- **Example**:
  ```
  GET /Mail/isValidEmail/test@example.com
  ```

### File Controller

Manages file operations with S3-compatible storage (DigitalOcean Spaces).

#### Endpoints

**GET** `/File/{bucketName}/getFileUrl/{fileName}`

Retrieves the public URL for a file stored in the specified bucket.

- **Parameters**:
  - `bucketName` (string, path): Name of the S3 bucket
  - `fileName` (string, path): Name of the file
- **Returns**:
  - `200 OK` - String containing the file URL
  - `500 Internal Server Error` - Error message if retrieval fails
- **Example**:
  ```
  GET /File/my-bucket/getFileUrl/image.jpg
  ```

**POST** `/File/{bucketName}/uploadFile`

Uploads a file to the specified S3 bucket.

- **Parameters**:
  - `bucketName` (string, path): Name of the S3 bucket
  - `file` (IFormFile, form-data): The file to upload
- **Request Size Limit**: 100 MB
- **Returns**:
  - `200 OK` - String containing the uploaded file name
  - `400 Bad Request` - If no file is provided
  - `500 Internal Server Error` - Error message if upload fails
- **Example**:
  ```
  POST /File/my-bucket/uploadFile
  Content-Type: multipart/form-data
  file: [binary data]
  ```

### String Controller

Provides string manipulation utilities including slug generation and string formatting.

#### Endpoints

**GET** `/String/generateSlug/{name}`

Generates a URL-friendly slug from the provided string.

- **Parameters**:
  - `name` (string, path): The string to convert to a slug
- **Returns**:
  - `200 OK` - String containing the generated slug
  - `500 Internal Server Error` - Error message if generation fails
- **Example**:
  ```
  GET /String/generateSlug/Hello World 123
  Response: "hello-world-123"
  ```

**GET** `/String/onlyNumbers/{input}`

Extracts only numeric characters from the input string.

- **Parameters**:
  - `input` (string, path): The string to extract numbers from
- **Returns**:
  - `200 OK` - String containing only numeric characters
  - `500 Internal Server Error` - Error message if extraction fails
- **Example**:
  ```
  GET /String/onlyNumbers/abc123def456
  Response: "123456"
  ```

**GET** `/String/generateShortUniqueString`

Generates a short, unique string identifier.

- **Returns**:
  - `200 OK` - String containing a unique identifier
  - `500 Internal Server Error` - Error message if generation fails
- **Example**:
  ```
  GET /String/generateShortUniqueString
  Response: "x4k9p2"
  ```

## Project Structure

```
NTools/
├── NTools.API/           # Web API layer with controllers and configuration
├── NTools.Application/   # Application layer with dependency injection setup
├── NTools.Domain/        # Domain layer with business logic and services
├── NTools.ACL/          # Anti-Corruption Layer for external services
├── NTools.DTO/          # Data Transfer Objects
└── NTools.Tests/        # Comprehensive test suite
```

## Development

### Prerequisites
- .NET 8.0 SDK
- Docker and Docker Compose
- MailerSend API account
- DigitalOcean Spaces (or S3-compatible storage) account`

## License

This project is licensed under the terms specified in the repository.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.