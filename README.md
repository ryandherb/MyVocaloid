# My.Vocaloid

Create your very own, simple vocaloid DAW in this project!

---

## How It Works

My.Vocaloid is comprised of three parts:

1) The Digital Audio Workstation (DAW)
2) The Sound Engine
3) The Voice Library

The DAW is the first step in the creation of a vocaloid track. Here, the user inputs their desired vocaloid sound formation. Each note in these vocaloid tracks is built from a Phoneme, duration and pitch. The Phoneme is the literal sound that should be made at that point (e.g. \b\ or \a\). Then, the user specifies how long that Phoneme should be played (up to 10 seconds). Lastly, the user pitches the Phoneme up or down depending on the desired sound.

Once the Track is created in the DAW, it is sent to the Sound Engine. Here, the inputs from the user are translated using NAudio to create a playlist of desired sounds. To create said sounds, we pull from the Voice Library, a collection of recorded Phonemes and modify those.

Lastly, the output is played for the user, resulting in a finished track.

---

### Information

Built by @ryandherb on gitub.
If you like this, check out my other projects. (:
