## Window Monitor

Checks for changes to foreground window handle and saves it to a file.

### Runs two concurrent threads. 

- First adds/updates Dictionary<IntPtr, string> when window change is detected
- Second checks for search term and if detected will replace it within dictionary

### Apps Sequence

1. Run
2. Pass Search Term default "Hello World!"
3. Pass Replacement Term default "Bingo"
4. Define if search is to be case sensitive default "No"
5. The App will monitor changes to foreground window app and log to console if found
6. Press "Q" to exit
7. Select if you want to save Window Titles to a file default "Yes"
8. Pass a filePath default "c:/users/{username}/WindowTitles.txt
9. Titles have been saved in as `{Handle as int}: {Title}`
