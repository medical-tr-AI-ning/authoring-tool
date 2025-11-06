# medical tr.AI.ning - Authoring Tool

<img width="2498" height="887" alt="banner" src="https://github.com/user-attachments/assets/0e34550f-a742-40e4-8212-94f8d3100ccf" />
<img width="2518" height="143" alt="contributors" src="https://github.com/user-attachments/assets/06d31904-d809-4182-b257-50dfe8cdb2d4" />

**medical tr.AI.ning** is a virtual reality training platform, designed for integration into the medical curriculum to enhance the clinical reasoning capabilities of future physicians.

The platform allows medical students to train clinical competencies from a first-person perspective with virtual, intelligent and interactive patients in an authentic simulated environment. Individual scenarios can be intuitively created by customizing specific parameters regarding patient, pathology and environment to support situated learning.

**This repository contains the Authoring Tool desktop application which is used to configure and export individually created medical scenarios to use in the VR Runtime**

[Read more about the project](https://medical-training-project.de/en/#project)

# Features

- Create **custom medical scenarios**, consisting of a VR environment, a patient character and an anamnesis interaction.
- Export scenarios to experience in VR (using the [VR Runtime](https://github.com/medical-tr-AI-ning/vr-runtime))
- Save and load presets for quick scenario creation.

## Environment Customization

- Choose between **multiple medical environments**.
- Equip the room with **medical items**.

<img width="1870" height="1080" alt="Authoring Tool Room Editing Screenshot" src="https://github.com/user-attachments/assets/f02acccc-a11c-4090-b385-43fabaa1ed24" />

## Patient Customization

- Define **personal details** to appear on the patient record in VR.
- Create realistic **skin pathology characteristics**, including melanoma and naevi.
- Write custom **answers to anamnesis questions** which the patient will respond with in the interactive dialogue.

<img width="1870" height="1080" alt="Authoring Tool Patient Editing Screenshot" src="https://github.com/user-attachments/assets/6221e735-537b-4c84-932c-bdf8b7c4ba21" />

# Usage

## Requirements

To run the medical tr.AI.ning Authoring Tool, the requirements are:
- System capable of running modern 3D applications
- Operating System: Windows 10 or newer (64 Bit)

## Setup

- Download the newest version from the [Releases](https://github.com/medical-tr-AI-ning/authoring-tool/releases) and extract the files.
- Once extracted, run `authoring-tool.exe`.

# Development

To set up the Unity project and make contributions to the software yourself, these steps must be followed:

## Requirements

- Install `Unity Editor 2022.3.62f2` using the [Unity Hub Application](https://unity.com/download)
- In case you don't have it yet, acquire a (free) Unity license in the Unity Hub Application

## Cloning the repository

This project uses common assets which are stored in a [separate submodule](https://github.com/medical-tr-AI-ning/common-assets).
To ensure that all required assets are present in the project, you need to clone the reposity using the `--recurse-submodules` option, e.g. 
`git clone https://github.com/medical-tr-AI-ning/authoring-tool.git --recurse-submodules`

## Workflow

- Import project folder into Unity Hub using `Add project from disk` and open the project
- The main scene to run the application and select scenarios is located at `Scenes/ScenarioList`
- The scenario configuration interface is located at `Scenes/ScenarioConfiguration`

<img width="3991" height="556" alt="sponsors" src="https://github.com/user-attachments/assets/c61efca6-6182-4ea9-9d36-694a2506dc79" />
