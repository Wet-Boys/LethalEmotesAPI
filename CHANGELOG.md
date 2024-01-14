- Version 1.1.16: Fixed cosmetic issues with MoreCompany/AdvancedCompany

- Version 1.1.15: Made the VR mod a soft dependency purely to let it load first since the IL code injections conflict otherwise

- Version 1.1.14: Fixed respawning in latecompany causing emotes to t-pose on the floor

- Version 1.1.13: Fixed VRM t-posing

- Version 1.1.12: Fixed quite a few bugs

- Version 1.1.11: Added enemy skeletons, do with that as you will

- Version 1.1.10: Probably fixed issue with morecompany cosmetics not being removed

- Version 1.1.9: Fixed incompat with fov mod. Fixed morecompany cosmetics not showing up (fixed? added)

- Version 1.1.8: Probably really fixed third person with the mirror mod for real this time.

- Version 1.1.7: Fixed issue with third person breaking when using the mirror mod. Updated integration with LethalConfig

- Version 1.1.6: removed some unncesssary debug logs. Added displayName as an animation parameter, this name will overrite the name that displays in the top left of the screen when emoting.

- Version 1.1.5: Probably fixed issue where you can't emote after being revived by mods that allow that. Fixed overrideMoveSpeed not working at all

- Version 1.1.4: Updated ModelReplacementAPI dll reference so it don't break. Exposed each bonemapper's audiosource in code if you should ever need it. Fixed networkobject hash on the networker being bad

- Version 1.1.3: Fixed an issue with the third person camera's culling mask sometimes showing purple hitboxes. Fixed items not sticking to your hands when emoting

- Version 1.1.2: Fixed an error with third person settings causing camera locking when using the hotswap button.

- Version 1.1.1: Added LethalConfig as a dependency cause it just makes sense

- Version 1.1.0: Added third person camera options, emotes can decide on their default, but you can override this in the emote customization menu, if LCThirdperson is installed, emotesapi will NEVER enter third person camera to avoid conflicts. Change the lock settings when joining other emotes, if you are an emote creator and have emotes that lock players to other players, please consider assigning BoneMapper.currentlyLockedBoneMapper when locking players (if you notice and don't like it, you can revert it in the config). Fixed healthbar not resetting its color on death. Probably fixed hitting escape in the wheel customization menu pulling up the pause menu.

- Version 1.0.4: Fixed a critical error where we didn't patch the networking calls into the DLL, optimized BoneMapper position syncing a bit

- Version 1.0.3: Fixed the need to disable the base animator when emoting, this means we touch the base stuff even less so there should be truly no compat issues with other emote mods. Fixed a bit of teleporting when you use more than one root motion emotes at once.

- Version 1.0.2: Actually did the position lock fix, oops.

- Version 1.0.1: Fixed some issues with setting samples for audio syncing. Fixed an issue with teleports interacting weird with position locks. Made the emote preview in settings a bit less jank.

- Version 1.0.0: Initial Release
