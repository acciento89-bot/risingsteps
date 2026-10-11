# Original background music — 11 October 2026

**A Path Through the Air** is an original eight-bar 4/4 composition at 84 BPM in D major. Its score, voicings, melodic phrases and synthetic instruments are authored in `scripts/compose-original-music.py`. No recorded samples, third-party tunes, downloads or external licenses are used. Full note-event provenance and WAV SHA-256 are in `docs/Audio/RisingAdventure.music.json`.

Each of the four games has a different original composition and arrangement. This game's music is intended to sit below gameplay cues, with a restrained .22 source gain, a .35-second fade and no spatialization. It spans 22.857 seconds and uses 22,050 Hz stereo PCM16 as the reproducible source. Unity imports Vorbis quality .55, compressed in memory, preloaded, with explicit 22kHz override; no runtime PCM allocation, synthesis or streaming occurs. Original PCM payload is 2,016,000 bytes. Engine compressed size and voice cost must still be measured in the integrated player.

Composition: soft piano broken chords, wood-like mallet accents and a breathy flute melody over gently changing pads; an airy walking/adventure phrase.

The render sums event/reverb tails circularly across the boundary rather than cutting them off or fading to silence. DC removal and RMS normalization preserve dynamics without a limiter. Measured normalized peak 0.479492, RMS 0.111997, DC 0.000000016, seam step 0.000549, seam slope change 0.000671; half-second RMS range 0.065268–0.161534. There are 95 authored note/rhythm events. This is musical phrasing rather than a continuous drone. Distinct hashes and waveform metrics for all four audition files are recorded in the delivery Music audit.

Reproduce with Python's standard library (no installation):

```sh
python3 scripts/compose-original-music.py --game rising --output Assets/Resources/Audio/RisingAdventure.wav
```

Move the generated `.music.json` into the matching `docs/Audio` location after regenerating. The composer can also render all four loops to an audition folder when `--game` is omitted.

Music preference is independent and persisted by the existing preference/profile owner. The existing Sound/AudioEnabled setting is the master mute, and wins immediately even while a Music fade is active. Music-only toggles fade; application pause and focus loss silence immediately through separate gates. Returning focus cannot override an active background pause. Re-entering the foreground fades back in. Time uses unscaled delta so opening an in-game menu does not interrupt the song. Existing gameplay SFX and haptics retain their behavior. The player owns one AudioSource, reuses it on repeated initialization and destroys it on teardown. Imported resource clips are shared assets and are never destroyed by a consumer.

Editor entry: `Kamilunavo.RisingSteps.Editor.OriginalMusicValidation.Validate`. It covers save migration/persistence and future-schema preservation, import bounds, PCM energy/seams, mute/fade, focus/background gating, missing resources, duplicate sources and owned-source/shared-clip lifetime. Source checks and PCM metrics do not establish real-device audible quality, compressed boundary behavior or native mixer headroom. Root must listen to actual playback beside SFX/engine sounds before audio acceptance. No build or TestFlight upload is authorized by this source handoff.

## Root integrated verification — 11 October 2026
Fresh actual Unity aggregate and local Mac development build passed with exit 0, including 42 music checks with Error/Exception/Assert capture. Evidence: `/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/rising-100-music-build4.log`. Component-only and whole-host teardown now dispose only owned sources and leave shared clips alive, using deferred editor cleanup to avoid destroying a source while its host is already being destroyed. Persisted music/master flags, imported loops, fade and focus/pause gates are exercised. Reported packaged audio asset size is 119.5 kB; this is not an iPhone runtime memory measurement.

Four intended-gain audio previews are in the delivery Music/previews folder, with gain/fade and source-hash provenance. They are auditions of the composed assets, not iPhone mixer recordings. Audible compressed seams, balance beside gameplay/engine cues and physical-device interruptions remain to be listened to on device. Graphical concept acceptance remains failed/pending. No new TestFlight upload or version bump was performed.
