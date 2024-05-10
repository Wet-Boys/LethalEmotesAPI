- Version 1.12.4: Fixed potentially showing LODs when multiple mods installed

- Version 1.12.3: Fixed issue where I was unintentionally relying on ModelReplacementAPI to fix first person arms for me

- Version 1.12.2: Added requireRaycastToJoin as an option for joinSpots. Basically just means you need line of sight to actually join the player. Doesn't work with world props, but I can look at it in the future if this ever becomes a problem.

- Version 1.12.1: Included debug symbols. Fixed sound collider causing issues with yeeting

- Version 1.12.0: Added Sentinel support

- Version 1.11.6: Fixed constant duplication of MoreCompany cosmetics

- Version 1.11.5: Slight optimization to the healthbar animator. Fixed issue with enemies spawning on clients

- Version 1.11.4: Now unlocks bones sooner upon player death to avoid conflicts with other mods

- Version 1.11.3: Made players less boulder-like

- Version 1.11.2: Fixed rare one-time bug upon launching with the new version

- Version 1.11.1: Fixed critical issue preventing terminal use. Fixed bunny ears not working in third person. Removed harmless error in the content creator detector

- Version 1.11.0: Made the DMCA options much more obvious if it detects you might be streaming. Added some tutorial text so players know about the join key. Added a few options to the config to make joining more obvious. Added emote skeletons for quite a few more modded enemies. Adjusted third person camera to be more accurate when pushing against walls. 

- Version 1.10.0: UI performanced significantly improved with large amounts of emotes, this will make the load time go from a few seconds to a few milliseconds when having more than a few emote mods installed. Added gamepad support, all bindings can be changed or removed if need be. Added toggles (default on) which allow BetterEmotes/TooManyEmotes to be populated into LethalEmotesAPI. These emotes are still tied to the configs of their respective mods (such as TooManyEmotes requiring emotes to be unlocked). But can be interacted with more fully, such as supporting the join button (default V). BetterEmotes emotes also supports the third person options in LethalEmotesAPI if used through LethalEmotesAPI methods.

- Version 1.9.1: Fixed issue where emote packs using the legacy import system weren't working nicely with keybinds

- Version 1.9.0: Added emote skeleton for the new snake boi

- Verison 1.8.2: Fixed keybinds being able to be used during areas they shouldn't, like the terminal

- Version 1.8.1: Fixed issue with third person camera clipping into walls with a select few emotes

- Version 1.8.0: Added keybind support. Specifically, in the customize menu you can now assign a keybind to any emote you want. Big thanks to Rune for making the UI work for this

- Version 1.7.5: Changed how we disable the hud to work better with other mods.

- Version 1.7.4: Added v50 enemies. This update is not required to work with v50 though.

- Version 1.7.3: Fixed issue with some enemies emoting in the ground due to new BoneMapper heirarchy changes

- Version 1.7.2: Minor fix around BoneRefs

- Version 1.7.1: Fixed scaling not being accurate (by about .1216 times) Added BoneRefs to all emote bones so that they can easily reference their counterparts

- Version 1.7.0: Quite a few new features! Global config is now a thing. Whenever you launch without a global config present, it will create one based off the current local config file, then after that it will be fully seperate, feel free to use either. The emote wheel will now tell you in more detail when you have emotes on it that are not currently installed. You can now join emotes of all kinds (unless the emote specifically says otherwise: CustomEmoteParams.allowJoining = false) not sure why I didn't have this be the case from the start but oh well. Your head during emotes will now be more alligned with the actual head, it was always kinda short as I've found, this is no longer the case. Fixed a few NREs when highlighting broken emotes in the customize menu. Reparented the BoneMapper so that crouching during an emote no longer moves you slightly backwards.

- Version 1.6.3: Fixed mod compat.

- Version 1.6.2: Fixed a longstanding issue with Start->Loop animations which caused them to not sync properly due to an overlap in the animation controller.

- Version 1.6.1: Fixed a rare issue with GrabbableObjectLateUpdate throwing errors. Move the bonemapper a tad bit in the heirarchy, fixing a future issue I noticed.

- Version 1.6.0: Added a local emote blacklist. By clicking the eye icon in the emote list, you can disable an emote for yourself only. Others will still be able to use and see the emote but you will not. Fixed an oversight with sync timers when an emote only syncs audio and not animations

- Version 1.5.2: Updated netcode patcher to 3.3.4, might help with ping related issues, idk ymmv

- Version 1.5.1: Updated some internal methods to be much more optimized. Should give a bit of a performance boost

- Version 1.5.0: Updated to use more accurate avatar for animations, consider using [the new ChadRig](https://github.com/Wet-Boys/LethalEmotesAPI/tree/main/ChadRigFolder) if you are animating your own emotes and you want more precision.

- Version 1.4.3: Enemy emote skeletons now get added a frame later to allow other mods to go first. Added BoneMapper.AttachItemHolderToTransform to allow easy locking of the held item position. Added a new import setting to prevent all movement during an emote.

- Version 1.4.2: Fixed a bug in realtion to advanced company not enjoying my emote bones existing. Made the deprecated importer sidestep duplicate emotes instead of straight up breaking when a duplicate is found

- Verison 1.4.1: Added emote skeleton for Peepers, added an option for the new import method to allow non-animating emotes

- Version 1.4.0: Added an emote skeleton for a few custom monsters

- Version 1.3.4: Fixed a weird 3-way bug with AdvancedCompany and More_Emotes when pulling out the portable terminal

- Version 1.3.3: Fixed local arms not being local enough when localTransforms is enabled on emotes.

- Version 1.3.2: Fixed syncing on emotes imported with the new system not working. Fixed oversight on MoreCompany cosmetic patches. Fixed when emotes get changed being a frame late

- Version 1.3.1: Removed debug commands, oops

- Version 1.3.0: Added new way to import emotes which allows more than one mod to have the same emote. This should address the issue where there are like 20 different emotes that are duplicated across multiple mods.

- Version 1.2.16: Fixed issue where we were breaking the bodycam mod

- Version 1.2.15: Upped LethalEmotesAPI priority in GameNetworkManager's Start but added a try catch block to attempt to sidestep some conflicts 

- Version 1.2.14: Fixed issue where I was destroying cosmetics when other mods still need them

- Version 1.2.13: Changed the third person camera to truly use the spectator camera's culling mask. This means it will only be able to see what you can see while spectating players (but it does mean if the spectator camera get's messed up, well... but this is better than the previous solution). Fixed issue when combing emotes from emotes api and toomanyemotes at the same time. The only conflict was when both played at the same time, so now emotes properly end when the other gets played to prevent this.

- Version 1.2.12: I suck

- Version 1.2.11: emergency fix oooooooooooooooooooooooooooooooooops

- Version 1.2.10: Hardened code on spawning to prevent conflicts with weird mod configurations

- Version 1.2.9: Fixed issue where I was locking the camera when I wasn't supposed to

- Verison 1.2.8: Fixed coil heads eating their own ass

- Version 1.2.7: Quick fix preventing spaghettification if your first emote has bones ignored from it's animation (literally no use case right now but just future proofing it)

- Version 1.2.6: Fixed audio sync issues, hopefully people will no longer be suffering randomly. Added useLocalTransforms to AnimationClipParams, this is primarily for if you are trying to do something like an upper body only animation where you  ignore the legs/pelvis

- Version 1.2.5: Fixed head not having proper mapping for emotes.

- Version 1.2.4: Fixed the first time any first person emote played, it would be camera locked.

- Version 1.2.3: Fixed being able to emoting during AC's terminal, fixed emote text not always showing up top

- Verison 1.2.2: Fixed a critical incompatibility issue I caused. Fixed first person arms not working when animating

- Version 1.2.1: Fixed the HealthbarAnimator from not spawning in the correct spot

- Version 1.2.0: Exposed the healthbar animator you can create and remove requests with HealthbarAnimator.StartHealthbarAnimateRequest() and HealthbarAnimator.FinishHealthbarAnimateRequest()

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
