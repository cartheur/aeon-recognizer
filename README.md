[![GitHub license](https://img.shields.io/github/license/cartheur/aeon-recognizer)](https://github.com/cartheur/aeon-recognizer/blob/main/LICENSE)
[![GitHub issues](https://img.shields.io/github/issues/cartheur/voice-recognizer)](https://github.com/cartheur/voice-recognizer/issues)

## A multi-tonality voice-recognizer for _aeon_ -- embodied as _henry_ and _david_

This project is a complete solution to having an emotional toy recognize a speaker and process their emotion via the recognition process. In order to use this software, you will need to build it from source. Before that, you will need to install some prerequisites some of which you may or may not have. The following instructions are for a Debian Linux system.

### Install prerequisites

```
     sudo apt install gcc automake autoconf libtool bison swig audacity libasound2-dev python-dev mplayer pulseaudio libpulse-dev
```
Leave the folder arrangment as it is set in this repository. It will make the build and install process easier.

### Building the sources

* Build sphinxbase

Note that only use `sudo` for installations, NOT for the building of the software. If you cloned from this repo, the file permissions might be incorrect. Navigate to the root folder and use `chmod -R 755 aeon-recognizer`. Next, step into to the sphinxbase folder and run the `autogen.sh` file:
		
```
    cd sphinxbase
    ./autogen.sh
```
Once completed and no errors are reported, run `make` and `make install` (this requires `sudo`)

```
    make
    sudo make install
```	
Check the installation by running the command `sphinx_lm_convert`. You may experience an error that sphinxbase cannot be found. If so, add path to the location where sphinxbase is installed `sudo nano /etc/ld.so.conf` and add new line `/usr/local/lib`. Refresh the configuration by using `sudo ldconfig`. Finally, then retest the installation `sphinx_lm_convert`. You will receive a message that it is missing arguments. This is the correct behavior at this stage.

* Build pocketsphinx

To begin, step into to the pocketsphinx folder and run the `autogen.sh` file:
		
```
    cd pocketsphinx
    ./autogen.sh
```
Once completed and no errors are reported, run `make` and `make install` (this requires `sudo`)

```
    make
    sudo make install
```	
Check the installation by running the command `pocketsphinx_continuous`. You may experience an error that `pocketsphinx_continuous` cannot be found. Since the path was already added in the previous step, just refresh the configuration `sudo ldconfig`. Finally, then retest the installation `pocketsphinx_continuous`. You will receive a message that it is missing arguments. This is the correct behavior.

## Using the recognizer

The last step is to copy the folders in the `model` directory (not copying the directory) to where the application can find them in the folder system. All that is needed is to run the application, for example, to STDOUT to the terminal a microphone input

```
pocketsphinx_continuous \
    -hmm /usr/share/pocketsphinx/hmm/en_US/en-us \
    -dict /usr/share/pocketsphinx/lm/cmudict-en-us.dict \
    -lm /usr/share/pocketsphinx/lm/en-us.lm.bin \
    -inmic yes
```
A `model` folder has been added to this repository. You can add and subtract models to change the consistency of the application. The live microphone is pretty nice and gives good results in a quiet environment with a good microphone, however, sometimes it is convenient to process a file. Using audacity gives good results by recording in mono with a project rate of 16000Hz. Export the `wav` file encoded in a 16-bit PCM format. Then use the command

```
pocketsphinx_continuous \
    -hmm /usr/share/pocketsphinx/hmm/en_US/en-us \
    -dict /usr/share/pocketsphinx/lm/cmudict-en-us.dict \
    -lm /usr/share/pocketsphinx/lm/en-us.lm.bin \
    -infile <filename>.wav
```

And the text will be output at the terminal. There is also the option to redirect STDOUT to a file. Further information on what you can do with the application can be found on this man page: https://www.mankier.com/1/pocketsphinx_continuous
	
### Using RabbitMQ to control the output of pocketsphinx recognition
	
When running the pocketsphinx application using the `-inmic` flag, it is not always convenient to use this uncontrolled in a recognizer application. Install RabbitMQ `apt install rabbitmq-server`.

### Some common issues

If you use this inside a VM, such as Debian 11 in VirtualBox, some of the audio settings for a USB microphone are not set correctly. When you select `Devices -> USB microphone` and test in audacity, things seem fine but you will need to install one more package to make it work correctly
```
sudo apt install osspd
```

### Improving the performance of the recongizer

Have added sphinxtrain to the repository. Documentation is found here: https://cmusphinx.github.io/wiki/tutorialam/

*Coming soon.*
