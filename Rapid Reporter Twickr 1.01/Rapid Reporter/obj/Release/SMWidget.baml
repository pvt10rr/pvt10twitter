   M S B A M L   `   `   `  ����   RapidReporter    Rapid_Reporter.SMWidget   \ XPresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    8x,http://schemas.microsoft.com/winfx/2006/xaml    -   $��SMWidgetForm��$��60��$��60��$��60��$	��700��$	��600��  � AllowsTransparency  .  � 
ResizeMode$ CanResizeWithGrip=� � Topmost .  � WindowStyle$
 None=� � Icon$ RapidReporter.ico�� � ContextMenu �� -   $��cm��.@�m� -   $��	TimerMenu��$��Time until end...��{�.  .@�m� -   $	��t60��$��60 min from now�� m� IsCheckable .  m� Icon �� $��icontimer.png��m� -   $	��t90��$��90 min from now�� .  �� $��icontimer.png��m� -   $
��t120��$��120 min from now�� .  �� $��icontimer.png��m� -   $��t0��$��Stop timer now!�� .  �� $��iconstopped.png�� �� $��icontimer.png��m� -   $��ChangeAccount��$��ChangeAccount��{�.   �� $��iconflick.png��$��16��m� -	   $��
OpenFolder��$��Open working folder��.@�m� -
   $��
FolderName��$�� �� �� $��iconsession.png��$��16��m� -   $��AboutBox��$��About...�� �� $��RapidReporter.ico��$��16��.��� 
����ح� � ColumnDefinitions �� $��20 ��� $��55 ��� $��* ��� $��25 �.+��� -   $��TransparencySlide��$��0��$��5q�$��10��	 � LargeChange$		 0.5Q�
 � Maximum$
 1Q� � Minimum$	 0.5Q� �� Orientation$ Vertical=� � Value$ 1Q� �� TickPlacement$ TopLeft=� �� TickFrequency$	 0.2Q� � ToolTip$ Transparency���� -   $��
ScreenShot��$��1��$��0,5,0,5q�$B <Snaps a screenshot. Shift+Click to edit and crop with Paint.��$	��Top=�$
��Left=�{�.   J� BitmapEffect ��  �� 
BevelWidth$ 0Q�.���� -   $��ScreenShotIcon��$��iconshot_dis.png��$��16��$��18��d� -   $��
RTFNoteBtn��$��1��$��0,5,0,5q� d� 	IsChecked$ FalseY�$ Opens the RTF note area��$��Bottom=�$
��Left=�{�.   �� $ 0Q� �� EdgeProfile$ BulgedUp=�.���� -   $��RTFNoteBtnIcon��$��iconnotes_dis.png��$��16��$��18��d� -   $��Flickr��$��1��$��0,5,0,5q�$ FalseY�$ Flickr Upload Disabled��$	��Top=�$��Right=�{�.  �� $ 0Q�$ BulgedUp=�.���� -   $��
FlickrIcon��$��iconflick_dis.png��$��16��$��18��d� -   $��Twitter��$��1��$��0,5,0,5q�$
 TrueY�$ Twitter Posting Enabled��$��Bottom=�$��Right=�{�.  �� $ 0Q�$ BulgedUp=�.���� -   $��TwitterIcon��$��icontwit.png��$��16��$��18��� -   $��ProgressBackground��$��2��$��5q���.  
����ح� � SmallChange$ 1Q��� -   $��nextType��$��2��$��9(�
������ ��.   �� TextAlignment$
 Left=�$	��120��$
��Left=�$	��Top=�$��5q�$��Center=�$
��Bold"�$��Arial)� J� 	AllowDrop .  |�.   �� 
IsReadOnly . $��0q� �� IsUndoEnabled .   �� MaxLines$ 1���� -   $��prevType��$��2��$��9(�
������ ��.  $
 Left=�$	��120��$
��Left=�$��Bottom=�$��5q�$��Center=�$
��Bold"�$��Arial)� .  |�.   . $��0q� .  $ 1���� -   $��NoteType��$��2��$��21(�
������ ��.  $ Right=�$	��130��$
��Left=�$��Stretch=�$��5,3,5,5q�$��Center=�$
��Bold"� .  |�.   . $��0q� .  $ 1���� -   $��NoteContent��$��2��$��21(�
������  . $��	135,3,5,5q�$��0q�$��Center=�.�� �� -   $��NoteHistory��$y�Hidden=�.���� -   $��FlickrInlogg��$��2��$�� ��$��9(�
������ ��.  $ Right=�$	��250��$��18��$��Right=�$	��Top=�$��Bottom=�$
��Bold"�$��Arial)� .  |�.   .  $��0q� .  $ 1���� -   $��currentTwitterAccount��$��2��$.��(Leave blank to disable Twitter Posting  ��$��9(�
������ ��.  $ Right=�$	��250��$��17��$��Right=�$��Bottom=�$	��Top=�$
��Bold"�$��Arial)� .  |�.   .  $��0q� .  $ 1���� -   $��
timeralarm��$��20��$��20��$��2��$��	0,10,10,0q�$��icontimer.png��$��Right=�$	��Top=�$y�Hidden=�$( "Time is up! Click here to dismiss.������  �� Triggers :�  :� RoutedEvent$ Image.Loaded��.[��� .���� .��W�  �� TargetProperty$ (Image.Opacity)� g� 	BeginTime$ 0:0:0X� g� Duration$ 0:0:0.5D�  W� From$	  1.0X�	! W� To$	! 0.0X�" g� RepeatBehavior$" Forever��# g� AutoReverse# . �� -   $��CloseButton��$��3��$��20��$��5q�$	��Top=�$ Finalize session��.��X�� -   $��charCounter��$��3��$��10(�$ Character Counter��$��Bottom=�$��32��
������ ��.  $ Center=�$	��Top=�$
��Bold"�
���   �$��Tahoma)� .  |�.   .  $��0q� .  $ 1��