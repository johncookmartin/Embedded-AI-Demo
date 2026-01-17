# Embedded AI Demo (Local LLM + .NET)

This repository is a simple demonstration of running a **local AI model** inside .NET applications using **LlamaSharp**.

The app loads a locally hosted version of **Llama 3.2** (running with Vulkan on a Radeon GPU) and uses it to **generate sample data from a provided JSON template**. This was built as a learning exercise to understand how local language models can be embedded into everyday software.

## What’s included

The solution contains three small test clients that all use the same underlying model setup:

- **WPF app:** A basic desktop interface where you can input a JSON template and receive generated sample data.  
- **ASP.NET API:** Exposes an endpoint that accepts a JSON template and returns AI-generated sample data.  
- **Console app:** A lightweight way to test prompts and outputs from the command line.

## Why this project exists

This project was created to explore:
- running AI models locally rather than in the cloud,  
- connecting an LLM to different types of .NET applications, and  
- experimenting with structured outputs based on JSON templates.

It is intended as a demo and learning project rather than a production system.
