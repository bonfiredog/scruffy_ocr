<img src="https://raw.githubusercontent.com/bonfiredog/scruffy_ocr/main/icons/window_icon.png" width="20%" height="20%" />

# scruffy_ocr
### A very scruffy handwritten note scanner for Windows.
<hr />

## Features

- Lightweight Windows application for digitising your handwritten notes.
- Uses the powerful [Microsoft Cognitive Services OCR](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/) for fast and accurate results.
- Save as plain text from existing scans or directly from your webcam.
- 'Burst Mode': Save multiple pages from a webcam with a customisable 'page turning' delay.

**Please note: this app requires a (free) subscription to [Microsoft Azure](https://portal.azure.com/#home), in order to use the Computer Vision resource that drives the handwriting recognition.**

<img src="https://github.com/bonfiredog/scruffy_ocr/blob/main/samples/sample_2.png" width="30%" height="30%" style="display:block; margin:0 auto" />

<hr />

## Setup

- Copy the `/Scruffy OCR` folder wherever you like. The Visual Studio project is in `/source`; other folders should be self-explanatory.
- Obviously I did not develop the recognition algorithms: instead I use the excellent Microsoft Cognitive Services API, available through Microsoft's [Azure](https://portal.azure.com/#home) platform. You will need to sign up for an Azure account, and then set up a new Computer Vision resource, making a note of the resource's generated **key** and **endpoint**. Instructions for doing so can be found at [the bottom of this page](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/#pricing). 
- Though an Azure account requires your bank details, the Free Tier provides 5000 calls to the API a month: more than enough for personal use.
- Opening the app for the first time will ask you to input your Cognitive Services **key** and **endpoint**. You can edit these at any time in the 'Settings' Tab.
- The 'From File' tab allows you to recognise handwriting from an image file, and then copy the recognised text or save it as a text file. You can set the preferred folder for this text file in the 'Settings' tab. 
- The 'From Camera' tab allows you to recognise handwritten text directly from the page, using your computer's webcam. You can do this with multiple pages by setting the number of pages you want to scan and the delay between camera captures. The recognised text will be automatically saved to a new text file in your preferred folder.

<hr />

## Why So Scruffy?

*I take a lot of handwritten notes, especially in the early stages of a project: however, the ideas that I generate in these notes almost invariably get sucked up into digital workflows. I used to spend a lot of time laboriously copying up these notes into digital form. Sometimes this remains a useful exercise: I rediscover insights that I had forgotten since the original note-taking, or I reconsider ideas that have become a little past their best.*

*However, sometimes I just want to get a lot of notes very quickly into my computer. Not much to ask, is it? However, there aren't many apps out there which do this:*

*a) for free;*

*b) well (i.e good recognition performance);*

*c) without a lot of proprietary nonsense.*

*So, I decided to improve my C#/WPF skills and build one myself. It is not beautiful, feature-rich or very elegant, but it does exactly what I need it to.*
