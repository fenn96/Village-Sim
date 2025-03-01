# LLM-Driven Village Simulation

A top-down, tile-based village simulation in Unity 6 where LLM-enhanced villagers exhibit realistic behaviors. The game features 25-30 villagers in a Stardew Valley-like setting with accelerated time. Players can observe autonomously functioning villagers or take direct/indirect control.

## Core Design Goals
- Create lifelike villagers using a hybrid of traditional AI and strategic LLM integration
- Implement psychological models (OCEAN, BDI, PAD) for realistic personalities
- Develop an efficient memory system that influences behavior
- Balance computational efficiency with behavioral complexity

## Development Setup
1. Unity 6.0.0 or newer
2. Required packages (see Package Manager)
3. OpenAI API key for LLM integration

## Project Structure
- Core systems can be found in the `_Project/Scripts/Core` directory
- Villager AI systems are in `_Project/Scripts/AI`
- The psychological framework is in `_Project/Scripts/Psychological`