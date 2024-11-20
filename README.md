# Towards Model-based Testing for Microservices

This repository contains the source code and experimental setup used for the Master's Thesis **"Towards Model-based Testing for Microservices"** by Bas van den Brink. The project implements and demonstrates a novel approach to model-based testing in microservice architectures using C# and .NET 6.

---

## Project Structure

### **`/src`**

This directory contains the core source code of the project. The code is written in **C#** and utilizes **.NET 6**. Key functionalities include:

- Model-based testing framework implementation.
- Tools for defining and executing tests based on Domain-Specific Languages (DSLs).

### **`/experiments`**

This directory includes a fork of Microsoft's **eShop on Containers**, a microservices-based reference application. It has been extended with additional directories ending in `.Experiments`, which contain:

- DSL files defining the models for testing.
- Templates and configurations necessary to execute the model-based testing process.

---

## Running the Proof of Concept

To execute the testing framework, use the **ProofOfConcept.Cli** project:

1. Navigate to the `src` directory and locate the `ProofOfConcept.Cli` project.
2. Build and run the CLI project using .NET tools:
   ```bash
   dotnet build
   dotnet run --project ProofOfConcept.Cli
   ```
3. For further instructions, refer to the source code of the CLI or the Master's Thesis for detailed guidelines on configuring and running the tests. There are additional steps required that have not been documented in this README yet.

## Prerequisites

.NET 6 SDK: Ensure you have .NET 6 installed. You can download it from Microsoft's .NET site.
eShop on Containers dependencies: The experiments rely on the dependencies provided in the original eShop on Containers repository.

## License

This project is licensed under the GNU General Public License (GPL). See the LICENSE file for more details.

## Acknowledgments

This project builds upon the eShop on Containers reference application by Microsoft and includes modifications to support model-based testing experiments.

For further context and theoretical foundations, please refer to the Master's Thesis: "Towards Model-based Testing for Microservices" by Bas van den Brink.
