@echo off
::dotnet dev-certs https -ep ./certs/cert.pfx -p 1234 -t
docker-compose build
docker-compose up %1