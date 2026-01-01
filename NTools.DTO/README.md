# NTools.DTO

[![NuGet](https://img.shields.io/nuget/v/NTools.DTO.svg)](https://www.nuget.org/packages/NTools.DTO/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

**NTools.DTO** is a comprehensive Data Transfer Objects (DTOs) library for the NTools utility services ecosystem. It provides strongly-typed models, settings, and request/response objects for various integrations and services.

## Features

### ChatGPT Integration DTOs
- `ChatGPTRequest` - Request model for ChatGPT API calls
- `ChatGPTResponse` - Response model with choices and usage statistics
- `ChatGPTMessageRequest` - Simple message request wrapper
- `ChatGPTErrorResponse` - Error handling models
- `ChatMessage` - Individual conversation message

### Email Service DTOs (MailerSend)
- `MailerInfo` - Email composition and sending
- `MailerRecipientInfo` - Recipient information
- `MailerErrorInfo` - Email service error handling

### Configuration Settings
- `ChatGPTSetting` - OpenAI API configuration (ApiUrl, ApiKey, Model)
- `MailerSendSetting` - Email service configuration
- `S3Setting` - S3-compatible storage configuration
- `NToolSetting` - General API settings

## Installation

```bash
dotnet add package NTools.DTO
```

## Usage

### ChatGPT Models

```csharp
using NTools.DTO.ChatGPT;

// Simple message request
var messageRequest = new ChatGPTMessageRequest
{
    Message = "What is the capital of France?"
};

// Full conversation request
var request = new ChatGPTRequest
{
    Model = "gpt-4o",
    Messages = new List<ChatMessage>
    {
        new ChatMessage { Role = "user", Content = "Hello" },
        new ChatMessage { Role = "assistant", Content = "Hi!" }
    },
    Temperature = 0.7,
    MaxCompletionTokens = 1000
};

// Handle response
var response = new ChatGPTResponse
{
    Id = "chatcmpl-123",
    Choices = new List<Choice>
    {
        new Choice
        {
            Message = new ChatMessage 
            { 
                Role = "assistant", 
                Content = "The capital of France is Paris." 
            }
        }
    }
};
```

### Configuration Models

```csharp
using NTools.DTO.Settings;

// ChatGPT configuration
var chatGPTSettings = new ChatGPTSetting
{
    ApiUrl = "https://api.openai.com/v1/chat/completions",
    ApiKey = "your-api-key",
    Model = "gpt-4o"
};

// Email service configuration
var mailSettings = new MailerSendSetting
{
    ApiUrl = "https://api.mailersend.com/v1/email",
    ApiToken = "your-token",
    MailSender = "noreply@example.com"
};

// S3 storage configuration
var s3Settings = new S3Setting
{
    AccessKey = "your-access-key",
    SecretKey = "your-secret-key",
    Endpoint = "https://your-bucket.s3.amazonaws.com"
};
```

### Email Models

```csharp
using NTools.DTO.MailerSend;

var email = new MailerInfo
{
    From = new MailerRecipientInfo 
    { 
        Email = "sender@example.com", 
        Name = "Sender Name" 
    },
    To = new List<MailerRecipientInfo>
    {
        new MailerRecipientInfo 
        { 
            Email = "recipient@example.com", 
            Name = "Recipient Name" 
        }
    },
    Subject = "Test Email",
    Html = "<h1>Hello World</h1>",
    Text = "Hello World"
};
```

## Dependencies

- **Newtonsoft.Json** (13.0.3) - JSON serialization/deserialization

## Key Features

- ✅ **Strongly Typed** - All models are type-safe with proper validation
- ✅ **JSON Annotations** - Proper JsonProperty attributes for API compatibility
- ✅ **OpenAI Compatible** - ChatGPT DTOs match OpenAI API specifications
- ✅ **Flexible Configuration** - Settings models support IOptions pattern
- ✅ **Error Handling** - Dedicated error response models
- ✅ **Well Documented** - XML documentation on all public members

## Compatible With

- **.NET 8.0**
- **ASP.NET Core**
- **OpenAI API**
- **MailerSend API**
- **AWS S3 / DigitalOcean Spaces**

## Related Packages

- **NTools.API** - REST API implementation
- **NTools.Domain** - Business logic and services
- **NTools.ACL** - Anti-Corruption Layer clients

## Repository

- **GitHub**: [https://github.com/landim32/NTools/tree/main/NTools.DTO](https://github.com/landim32/NTools/tree/main/NTools.DTO)
- **Issues**: [https://github.com/landim32/NTools/issues](https://github.com/landim32/NTools/issues)

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/landim32/NTools/blob/main/LICENSE) file for details.

## Author

- **Rodrigo Landim** - [Emagine](https://github.com/landim32)

## Support

For questions, issues, or contributions, please visit the [GitHub repository](https://github.com/landim32/NTools).
