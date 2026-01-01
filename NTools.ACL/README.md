# NTools.ACL

[![NuGet](https://img.shields.io/nuget/v/NTools.ACL.svg)](https://www.nuget.org/packages/NTools.ACL/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Overview

**NTools.ACL** (Anti-Corruption Layer) is a client library that provides strongly-typed HTTP clients for consuming NTools API services. It simplifies integration with NTools APIs by offering clean interfaces, dependency injection support, and built-in logging.

## Features

### Available Clients

- **ChatGPTClient** - ChatGPT/OpenAI integration
- **MailClient** - Email validation and sending (MailerSend)
- **FileClient** - File upload and URL retrieval (S3-compatible storage)
- **DocumentClient** - CPF/CNPJ validation
- **StringClient** - String manipulation utilities

### Key Benefits

- ✅ Strongly Typed - Type-safe API calls with IntelliSense support
- ✅ Dependency Injection - Full DI support with IHttpClientFactory
- ✅ Logging - Integrated logging with ILogger
- ✅ Configuration - Uses IOptions pattern for settings
- ✅ Error Handling - Automatic HTTP error handling
- ✅ JSON Serialization - Built-in JSON handling

## Installation

```bash
dotnet add package NTools.ACL
dotnet add package NTools.DTO
```

## Configuration

### appsettings.json

```json
{
  "NTool": {
    "ApiUrl": "https://your-ntools-api.com"
  }
}
```

### Startup.cs / Program.cs

```csharp
services.Configure<NToolSetting>(Configuration.GetSection("NTool"));
services.AddHttpClient<IChatGPTClient, ChatGPTClient>();
services.AddHttpClient<IMailClient, MailClient>();
services.AddHttpClient<IFileClient, FileClient>();
```

## Usage Example - ChatGPT

```csharp
public class MyService
{
    private readonly IChatGPTClient _chatGPTClient;

    public MyService(IChatGPTClient chatGPTClient)
    {
        _chatGPTClient = chatGPTClient;
    }

    public async Task<string> AskQuestion(string question)
    {
        return await _chatGPTClient.SendMessageAsync(question);
    }
}
```

## Repository

GitHub: [https://github.com/landim32/NTools/tree/main/NTools.ACL](https://github.com/landim32/NTools/tree/main/NTools.ACL)

## License

MIT License - Copyright (c) Rodrigo Landim / Emagine

