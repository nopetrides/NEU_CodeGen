# AI Agent Coding in Unity

## Overview

This project explores the effectiveness of AI agents in assisting with Unity development. Specifically the creation of a UI menu using Unity's UI Toolkit, UXML, and USS. The goal was to determine whether an AI-assisted workflow could reduce setup time and automate repetitive tasks.

## Setup and Tools

Unity Hub was installed and configured with Unity 6. A new project was created for testing. The primary AI tool used was **Cursor**, which functions as a VSCode-based IDE and supports VSCode extensions. After installing the Unity VSCode extension within Cursor, integration with Unity was configured via `Edit > Preferences > External Tools > External Script Editor > Cursor.exe`. Cursor was then used to open and edit the Unity project.

## Workflow Test

To begin, the AI was asked to integrate [**Rosalina**](https://github.com/Eastrall/Rosalina), a third-party Unity plugin. Cursor not only imported the package successfully but also created a functional UI menu without being explicitly instructed to do so. See Exhibit A for the very first prompt of the project. This provided key insights into how Agents work and how much they are able to accomplish with minimal input.

Manual code changes introduced a caveat: Cursor would sometimes overwrite these during subsequent steps, showing limited awareness of manual edits. After roughly three hours, the following system was operational: Boot > GameManager > UIManager > MainMenu, complete with button interactivity and audio feedback.

Eventually, API usage limits were hit, prompting a shift toward local AI solutions and reviewing other candidates.

## Local AI Alternatives

Local models were tested to bypass usage limits. **Ollama with Qwen2.5-Coder:32b** was installed following [this guide](https://glucn.com/posts/2025-02-16-use-local-deepseek-in-cursor). This setup allowed continued use without API caps but resulted in diminished functionality such as loss of broader codebase awareness.

Other tools were also evaluated. **JetBrains AI Assistant**, an LLM with contextual code awareness, worked well and supported local execution. **Junie**, another agent similar to Cursor, could not be run locally at the time of testing.

## Key Findings

AI agents tend to overperform, often taking initiative beyond the original prompt. This seems to be a result of how agents break tasks down internally. A primary LLM callback generates a plan, and others execute each step. While helpful in larger workflows, this behavior can become intrusive for precise or minimal changes and has hugely inefficient API cost to results. A single line change could involve 9 API calls.

Syntax handling was another weak point. Despite explicit instructions to use Unity USS, the AI would occasionally generate unsupported CSS syntax, causing compile errors. Additionally, the agents were inefficient when correcting their own minor mistakes—overhauling entire instruction chains to fix what amounted to a typo or missed step. For small tasks, AI Assistant proved more practical, while Junie or Cursor worked better for multi-step setups.

Agents worked best when tasks could be completed entirely through code. Interactions requiring Unity Editor input (like linking UXML files in the inspector) often confused the AI, sometimes requiring the user to halt or rollback the process.

Contextual inconsistency was frequent. Though the AI claimed to reference the full codebase, it often ignored existing scripts or duplicated logic unless explicitly instructed to follow established patterns. When dealing with niche tools like the **Pixel Crushers Dialogue System**, the AI was effectively blind unless given specific code snippets, showing poor exploratory capabilities. It performed well with general Unity tasks but degraded in utility as specificity increased.

Agents are also able to execute terminal commands on the user's machine. Though they require explicit permission to be run, Junie has a "Brave Mode" that allows the AI to execute whatever commands it deems necessary without pause or user input.


## Conclusion

AI-assisted development in Unity shows promise, particularly for setting up systems, automating repetitive boilerplate, or generating entire manager structures. However, current agents require careful supervision, especially when involving manual editor steps or third-party plugin integration. The most efficient workflow at present appears to be a hybrid approach: using agents like Cursor or Junie for full workflows and JetBrains AI Assistant for quick, isolated code assistance. Sticking to code-only operations and minimizing reliance on Editor-based steps produces the most reliable results.
