# Resumen del Proyecto: Anti-Corruption Layer Pattern
Este es un proyecto de ejemplo que implementa el patrón Anti-Corruption Layer (ACL) durante la migración de una aplicación monolítica ASP.NET a microservicios en AWS.

# ¿Qué es el Anti-Corruption Layer?
Es una capa de traducción que actúa como intermediaria entre dos sistemas con diferentes modelos de dominio. Su objetivo es:
*1.	Traducir semántica: Convierte el modelo del monolito a la del microservicio
*2.	Aislar cambios: El código que llama no necesita cambios cuando la microservicio evoluciona
*3.	Reducir riesgo: Minimiza la disrupción durante la migración de monolito a microservicios

# Arquitectura del Proyecto

┌─────────────────┐
│  Program.cs     │  (Aplicación monolítica)
│    (Caller)     │
└────────┬────────┘
         │ Llama a
         ▼
┌─────────────────────────────────┐
│  UserInMonolith                 │  (Interfaz original del monolito)
│  - UpdateAddress()              │
└────────┬────────────────────────┘
         │ Delega a
         ▼
┌──────────────────────────────────┐
│  UserServiceACL (Anti-Corru...)  │  ⭐ CAPA DE TRADUCCIÓN
│  - Traduce modelos               │
│  - Llama a la API Gateway        │
└────────┬─────────────────────────┘
         │ Invoca
         ▼
┌──────────────────────────────────┐
│  AWS API Gateway                 │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│  AWS Lambda (Microservicio)      │
└──────────────────────────────────┘

# Estructura de 3 Repositorios

Git Repository: anti-corruption-layer-pattern (origin)
├── ✅ anti-corruption-layer-impl (Monolito)
├── ✅ cdk-user-microservice (Infrastructure)
└── ✅ user-microservice-lambda (Microservicio .NET 8.0)

# Flujo de Traducción

*Modelo del Monolito (UserDetails):
UserId: 12345
AddressLine1: "475 Sansome St"
AddressLine2: "10th floor"
City: "San Francisco"
State: "California"
ZipCode: "94111" (string)
Country: "United States"

*Modelo del Microservicio (UserMicroserviceModel):
UserId: 12345
Address: "475 Sansome St, 10th floor"  // ⭐ Combinado
City: "San Francisco"
State: "California"
ZipCode: 94111                          // ⭐ Convertido a int
Country: "United States"

*Traducción en UserServiceACL.cs:
// Combina dos líneas de dirección en una
userMicroserviceModel.Address = userDetails.AddressLine1 + ", " + userDetails.AddressLine2;

// Convierte string a int
Int32.TryParse(userDetails.ZipCode, out int zipCode);
userMicroserviceModel.ZipCode = zipCode;