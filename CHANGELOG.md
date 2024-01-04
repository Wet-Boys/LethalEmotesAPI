- Version 1.1.1: Added LethalConfig as a dependency cause it just makes sense

- Version 1.1.0: Added third person camera options, emotes can decide on their default, but you can override this in the emote customization menu, if LCThirdperson is installed, emotesapi will NEVER enter third person camera to avoid conflicts. Change the lock settings when joining other emotes, if you are an emote creator and have emotes that lock players to other players, please consider assigning BoneMapper.currentlyLockedBoneMapper when locking players (if you notice and don't like it, you can revert it in the config). Fixed healthbar not resetting its color on death. Probably fixed hitting escape in the wheel customization menu pulling up the pause menu.

- Version 1.0.4: Fixed a critical error where we didn't patch the networking calls into the DLL, optimized BoneMapper position syncing a bit

- Version 1.0.3: Fixed the need to disable the base animator when emoting, this means we touch the base stuff even less so there should be truly no compat issues with other emote mods. Fixed a bit of teleporting when you use more than one root motion emotes at once.

- Version 1.0.2: Actually did the position lock fix, oops.

- Version 1.0.1: Fixed some issues with setting samples for audio syncing. Fixed an issue with teleports interacting weird with position locks. Made the emote preview in settings a bit less jank.

- Version 1.0.0: Initial Release
