# Copilot Instructions for FindAGunDamnIt! (Continued) Mod Development

## Mod Overview and Purpose

**FindAGunDamnIt! (Continued)** is a RimWorld mod initially developed by AliceTries, now updated and maintained. The core aim is to streamline the process by which pawns, particularly hunters, autonomously locate and equip weapons without player intervention. The mod ensures that emergencies like the "hunter lacks a weapon" alert are addressed efficiently, allowing for a smoother gameplay experience.

The update focuses on enhancing compatibility with other mods while refining the weapon selection mechanics.

## Key Features and Systems

- **Automatic Weapon Search**: Pawns will autonomously search for and equip appropriate weapons, reducing manual oversight.
- **Mod Compatibility**: 
  - Integrates with the Simple Sidearms and Android Tiers mods.
  - Known incompatibilities with Awesome Inventory.
- **Customization Options**: 
  - Set preferences for weapon selection based on accuracy type (long/medium/short/close range).
  - Option to enable or disable certain features for better control.
- **Job Giver Extension**: Extends `JobGiver_PickUpOpportunisticWeapon` to automate weapon acquisition.
- **Thought Tree Integration**: Modifies pawn thought processes to prioritize weapon procurement within their outfits.

## Coding Patterns and Conventions

- **Code Structure**: Follows a modular pattern with distinct responsibilities for each class.
- **Naming Conventions**: 
  - UpperCamelCase for classes and methods.
  - camelCase for variables.
- **Error Handling**: Incorporate logging for debugging, especially in areas concerning mod interactions.

## XML Integration

- **Patch Files**: Two XML files (`PatchThoughts.xml`, `PatchThoughts_AndroidTiers.xml`) are used to integrate modifications with the game's thinking and thought tree logic.
  - Ensure values and nodes in XML align with game definitions.
  - Maintain detailed comments within XML files to document changes and interactions.

## Harmony Patching

- **Purpose**: Utilized for runtime modification of existing game methods.
- **Approach**: 
  - Apply postfix and prefix patches where needed, specifically in areas altering outfit functionality and job delivery.
  - Ensure patches are only applied when certain mods (like Android Tiers) are active to prevent unwanted behavior.

## Suggestions for Copilot

- **Tool Usage**: When using Copilot, focus on suggestions for repetitive tasks like method stubs, try-catch blocks, and interface implementation.
- **Contextual Suggestions**: Since modding involves working within an existing game framework, leverage Copilot to propose code by learning patterns from Harmony patches and RimWorld API usage.
- **Testing**: Harness Copilot to design testing methods that ensure mod compatibility and correctness, especially during weapon assignment and behavior changes.
- **Documentation**: Use Copilot to help draft and refine comments and documentation blocks in both C# and XML files to maintain readability and future maintenance.

By following these guidelines and utilizing Copilot effectively, contributors can efficiently develop and improve the FindAGunDamnIt! mod, enhancing gameplay for RimWorld users.


This document provides a comprehensive set of guidelines and instructions for developers working on the FindAGunDamnIt! mod project, enabling efficient use and integration of GitHub Copilot into their workflow.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).

