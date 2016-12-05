#Runtime Object Editor
An updated version of https://www.codeproject.com/articles/14292/runtime-object-editor

===============
ORIGINAL BELOW
---------------

Runtime Object Editor
Developed by:
Corneliu I. Tusnea
www.acorns.com.au 

Usage:
1. To Attach to other .Net processes:
	Run RuntimeObjectEditorLoader\bin\Release\RuntimeObjectEditorLoader.exe
		
2. To Include it in you application:
	Add a reference to RuntimeObjectEditor.dll. 
	Add RuntimeObjectEditor.ObjectEditor.Instance.Enable(); in your static void Main just before your Application.Run code.
	This will enable the ObjectEditor and will register the hotkey.
	The default hotkey used is "Control+Shift+R".
	If you want to change the default hotkey, just set RuntimeObjectEditor.ObjectEditor.Instance.HotKey to whatever key combination you want.
	The property supports combinations like: Control|Alt|Shift|+key. (e.g.: Control+Shift+Alt+F1). 
	When running your application, press the hot key combinations you registered and start playing with the editor.
		
Enjoy,
Corneliu.

www.acorns.com.au
