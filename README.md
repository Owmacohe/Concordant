![License: MIT](https://img.shields.io/badge/License-MIT-yellow)
![GitHub stars](https://img.shields.io/github/stars/owmacohe/concordant)
![GitHub downloads](https://img.shields.io/github/downloads/owmacohe/concordant/total)

![Concordant logo](Media/Logo/Concordant_Logo_200.png)

# Concordant

*con • cord • ant*

> 1. in agreement with other facts.
> 2. based on the same principles as something else.
> 3. harmonious.

## Overview

**Concordant** is a simple yet powerful text localization manager. Create, modify, export, and import databases of terms, sentences, or any other localizable text into any system language. In-game dialogue updates immediately, as soon as the current language is changed, for immediate hot-swapping results. **Concordant** aims to capture the core principles and features of larger localization managers, without succumbing to feature-bloat or indecipherable UIs.

## Installation

1. Install the latest release from the [GitHub repository](https://github.com/Owmacohe/Concordant/releases), unzip it, and place the folder into your Unity project's `Packages` folder.
2. Return to Unity, and the package should automatically be recognized and visible in the **Package Manager**.
3. A sample scene can be found at: `Concordant/Example/Example.unity`.
4. Opening this scene may prompt you to install **Text Mesh Pro**. Simply click on **Import TMP Essentials** to do so.

## Usage

1. All localized text is stored as entries in a localization database. The databases are `ScriptableObject`s, and can be created with `Create/Concordant Database`.
2. Rename your newly created database, then add any number of languages to it using the **Inspector** window. Then right click on it and select `Edit Concordant Database`. This will open the **Concordant Database Editor** window.
3. To add new entries to the database, simply write your desired **ID** and **category** in the input row, and click the **+** button.
   - **IDs** represent an entry's name, and **categories** represent the part of the game or UI an entry pertains to. Entries can have the same **ID**, so long as their **categories** are different, and vice versa.
   - Together, an entry's **category** and **ID** make up its **key** within the database, which is used to reference it later. A key is formatted as **CATEGORY/ID**. 
   - Once an entry has been added, its **ID** and **category** can be changed at any time, so long as it wouldn't result in a duplicate **key** in the database.
5. Click on the **v** button to the right of the entry to expand it. From here, you can add the translation text for any of the languages you added in step 2. You can also add a context in which the term normally appears in-game, to aid translation in the future.
6. While the editor does perform some autosaving, don't forget to click on the **Save** button to be absolutely sure.
7. For easier translating, you can export the entire database as a `.csv` file. Simply click on the **Export** button and choose a  file destination at which to save it. This file can then be opened in any spreadsheet software, and edited line by line.
8. When this process is complete, click on the **Import** button and select your edited `.csv` file. All changes to translations and context fields should now appear in the database. Don't forget to **Save** after!