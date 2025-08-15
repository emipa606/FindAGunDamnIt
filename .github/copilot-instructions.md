# GitHub Copilot Instructions for "FindAGunDamnIt! (Continued)" Mod

## Mod Overview and Purpose

"FindAGunDamnIt! (Continued)" is a mod for RimWorld that aims to automate the process of equipping pawns with weapons, especially addressing the frustration of hunters standing idly by a weapon. The mod extends job assignment systems and introduces conditional logic to enhance the weapon-selection process. It offers several integration options with other popular mods and provides an adjustable configuration to better suit various gameplay styles.

## Key Features and Systems

- **Automated Weapon Selection**: Pawns will automatically find and equip weapons without player input, particularly focusing on efficient hunter management.
- **Custom Weapon Search Settings**: Configurable options are available to determine how pawns search for weapons, including selecting weapons with similar accuracy types (long/medium/short/close range).
- **Mod Integration Support**:
  - Simple Sidearms: Ensures compatibility and enhances weapon handling.
  - Android Tiers: Specifically addresses weapon management for Android pawns.
- **Compatibility Issues**: Known issues with the "Awesome Inventory" mod.
- **Thought Tree Integration**: Patches the thought process of pawns to logically select weapons available in the environment.

## Coding Patterns and Conventions

- **Naming Conventions**: Classes and methods use PascalCase, while variables use camelCase.
- **Access Modifiers**: Key classes are internal to limit unnecessary exposure, while others are public as needed.
- **Static Classes**: Used for utility operations related to outfits and weapons.
- **Job Logic Extension**: `JobGiver_PickUpOpportunisticWeapon_Extended` inherits and extends the base logic for picking up weapons.

## XML Integration

- XML is used to define mod settings and default configurations.
- Configurable settings allow players to tailor weapon search behavior directly within RimWorld's mod settings interface.

## Harmony Patching

- Harmony is employed to patch and extend RimWorld's base classes:
  - Extend job givers to introduce enhanced weapon-seeking behavior.
  - Adjust the thought tree logic to consider weapon availability and outfit policies.

## Suggestions for Copilot

1. **Class Extensions**: When creating new systems or extending existing RimWorld behavior, ensure new classes and methods are appropriately integrated into the mod framework, paying attention to naming conventions and accessibility.
   
2. **Harmony Patches**: Use Copilot to assist in writing and optimizing Harmony patches by suggesting detour and postfix methods to modify base game logic.

3. **Config Settings**: When adding new configurable settings, suggest methods for both saving and loading these settings consistently with existing patterns in `FindAGunDamnItModSettings`.

4. **Error Handling**: Implement robust error handling by catching exceptions and logging them where necessary, ensuring smoother integration with other mods.

5. **Debugging Features**: Suggest adding debug logging to track when and how pawns select weapons, especially useful in troubleshooting compatibility issues.

By following these instructions and guidelines, contributors can efficiently expand and maintain the "FindAGunDamnIt! (Continued)" mod, ensuring it remains a helpful tool for RimWorld players seeking to automate weapon management for their pawns.
