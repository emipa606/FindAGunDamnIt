# GitHub Copilot Instructions for FindAGunDamnIt! (Continued) Mod

## Mod Overview and Purpose

**Mod Name**: FindAGunDamnIt! (Continued)

**Description**: This mod is an update of the original mod by AliceTries. It aims to simplify the process of pawns acquiring weapons in RimWorld, especially for roles like hunters. The mod automatically equips pawns with nearby weapons, addressing the frustrating alert when a hunter lacks a weapon while standing on one.

## Key Features and Systems

- **Weapon Acquisition Settings**: Customize how pawns should search for weapons, ensuring they always have access to the appropriate tools for their tasks.
- **Mod Compatibility**: Supports integration with other popular mods such as Simple Sidearms and Android Tiers, but not compatible with Awesome Inventory.
- **Accuracy-Type Switching**: Provides an option to switch to weapons with similar accuracy types (e.g., long, medium, short, close), optimizing pawn weapon choices.
- **Job Extensions**: Extends the job giver systems to improve how pawns select weapons based on their environment and tasks.

## Coding Patterns and Conventions

- **Class Structure**: The main mod functionality is divided into several classes, with each class focusing on a specific aspect of the mod’s functionality.
- **Static Methods and Classes**: Utilizes static methods and classes (`Gunfitter`, `GunsInOutfits`) to manage shared logic and reduce redundancy.
- **Internal Classes**: Certain classes like `FindAGunDamnItMod` and `FindAGunDamnItModSettings` are internal, allowing easy access within the project while encapsulating functionality.
- **Inheritance**: The class `JobGiver_PickUpOpportunisticWeapon_Extended` inherits from `JobGiver_PickUpOpportunisticWeapon` to extend functionality without altering existing game logic.

## XML Integration

The mod requires careful integration with RimWorld's XML definitions:
- Ensure XML definitions correctly specify any new or altered job definitions.
- Use XML for defining settings and options that can be configured by the user.

## Harmony Patching

- **Patching Strategy**: Use Harmony to patch original methods to introduce new functionalities without modifying core game code. This ensures compatibility and reduces the risk of conflicts.
- **Example Patch Usage**: Extend and override methods in the `Thought Tree` and `Job Drivers` where necessary.

## Suggestions for Copilot

- **Improve Accuracy-Type Logic**: Assist in refining the logic for switching weapons based on accuracy type. Consider edge cases where pawns have multiple weapon alternatives nearby.
- **Simplify Mod Integration**: Provide code suggestions to enhance compatibility with more mods while troubleshooting potential conflicts automatically.
- **Error Handling**: Generate robust error-handling code sections that log discrepancies and provide feedback for users to upload logs using the Log Uploader tool.
- **Debugging Tools**: Suggest automated testing strategies or snippets to verify the correct functioning of the extended job giver logic and compatibility features.

Remember to test any changes in a mod development environment, and always check for updates from dependencies before deploying any modifications. For issues or further support, use the designated Discord channel or GitHub repository discussions, avoiding GitHub threads which may not send notifications.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
