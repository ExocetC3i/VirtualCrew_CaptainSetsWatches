# Remove First Officer Requirement

A BepInEx 5 plugin for SailwindVirtualCrew. It resolves `VirtualCrewManager.HasFirstOfficer` from the loaded SailwindVirtualCrew assembly and applies a Harmony postfix that forces the result to `true`.

Every SailwindVirtualCrew code path that reads `HasFirstOfficer` will see a First Officer as present, even when no First Officer is aboard. The plugin verifies that Harmony registered its postfix and reports an error if the target or patch is unavailable.

## Build and install

Build `RemoveFirstOfficerRequirement.csproj`. The build creates `RemoveFirstOfficerRequirement.zip` in the project directory with `RemoveFirstOfficerRequirement.dll` inside a `RemoveFirstOfficerRequirement` folder. Extract that folder into Sailwind's `BepInEx\plugins` directory. SailwindVirtualCrew must also be installed.
