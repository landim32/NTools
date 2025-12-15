# NTools API - Docker Guide

## Comando para subir o SQL Server no docker
```
docker compose -f .\sql_server_compose.yml up -d
```

## Comandos para subir a API NTools

### Build da imagem Docker
```
docker compose build
```

### Subir o container
```
docker compose up -d
```

### Ver logs do container
```
docker compose logs -f ntools-api
```

### Parar o container
```
docker compose down
```

### Rebuild completo (limpar cache)
```
docker compose build --no-cache
docker compose up -d
```

## Acessar a API
Após subir o container, a API estará disponível em:
- http://localhost:5000
- Swagger UI: http://localhost:5000/swagger

## Deploy na Azure
```
az container create --resource-group GoblinWarsRecursos --file deployPodsAz.yml
```

## Check Deploy
```
az container show --resource-group GoblinWarsRecursos --name goblin-wars --output table
```

## Registry Log
```
az acr login --name registrygw