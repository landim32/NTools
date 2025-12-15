#!/bin/bash
docker stop ntools-api1
docker rm ntools-api1
cd ./Backend/NTools
docker build -t ntools-api -f ./NTools.API/Dockerfile .
docker run --name ntools-api1 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORTS=443 --network docker-network ntools-api &
docker ps
