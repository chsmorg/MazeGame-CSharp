   M S B A M L   `   `   `  ����C  ?MazeGame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:MazeGameMazeGame  5   	       MazeGame.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:MazeGame     5   	     � Title$  
MainWindow��5   	   $
��1000��6   $
��1000��6*    � 	Resources 5	      3  -System.Windows.Media.Effects.DropShadowEffect   
ButtonGlow   Color   ShadowDepth   
BlurRadius   Opacity��%O   &	          5
   
   $
 Gold��6.   $ 0Q�6;   $ 5Q�6K   $ 1Q�6Z   5      .��5      ]� .+�5   
   o� $	��Top=�6   .@�5      m� $��
Difficulty��6   .@�5      m� -   5         m�Name$ EasyMenuItem��5      $
��Easy��61   6?   m� 5      -   5        $ MediumMenuItem��5      $��Medium��63   6C   m� 5      -   5        $ HardMenuItem��5      $
��Hard��61   6?   5      m� 5      -   5        $ StartMenuItem��5      $��Start��6.   6=   m� 5      -   5        $ RestartMenuItem��5      $��Restart��60   6A   m� 5      -   5        $ CustomSizeMenuItem��5      $��Custom Size��63   .@�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� -   5         ��Name$ CustomSizeTextBox��5      $	��100��69   �� 5      -   5        $��Submit��5      6.   5      5      m� 5      -	   5        $ TimerMenuItem��5      $��00:00��6.   m� 5      -
   5        $ KeyModeMenuItem��5      $��Key Mode��60   6B   m� 5      -   5        $ SolveMenuItem��5      $��Solve��6.   6=   5      � 5   
   	 � RowDefinitions	 5       �� 5!      $��* �6    �� 5"      $
��Auto �6    �� 5#      $��* �6    
 � ColumnDefinitions
 5%      �� 5&      $��* �6#   �� 5'      $
��Auto �6#   �� 5(      $��* �6#   .+�5*      �� -   5         ��Name$ 
MazeCanvas��5*      $��1��6)   $��1��66   5+      5-      5.      