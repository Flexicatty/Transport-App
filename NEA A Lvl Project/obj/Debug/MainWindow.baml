   M S B A M L   `   `   `  ����L  HNEA A Lvl Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:NEA_A_Lvl_ProjectNEA_A_Lvl_Project  5   	   "    NEA_A_Lvl_Project.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:NEA_A_Lvl_Project     5   	   M IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=nullD @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	   -   5        $��mainWin��5
   5   0  *ModernWpf.Controls.Primitives.WindowHelper    UseModernWindowStyle  .  � Title$ 
MainWindow��6	   $	��450��6   $	��800��6)    � WindowStartupLocation$ CenterScreen=�6D   .��5      �� -   5        $	��myC��5      .+�5   
   �� -   5         ��Name$ loginbutton��5      $��Login��6&   $
��Left=�66   $��34��6Q   $	��Top=�6]   $��90��6u    �� Left$	 702��6�   
���   �$	��1.5q�6�   �� 5   
   -   5        $ trainButton��5      $	��130��6&   $ 71��63   
 �� Top$	 138��6D   $	��160��6U   $��20(�6a   $��SemiBold"�6o   
���   �.��5      �� -   5        $��	traintext��5      $
��Wrap=�6)    �� TextAlignment$ Center=�6=    J� Opacity$ 0Q�6T   Train Timetable6q   5      �� 5   
   -   5        $ 	busButton��5      $	��130��6$   $	 236��61   $	 138��6C   $	��160��6T   $��20(�6`   $��SemiBold"�6n   
���   �.��5      �� -   5        $��bustext��5      $
��Wrap=�6'   $ Center=�6;   $ 0Q�6R   Bus Timetable6m   5      �� 5   
   -   5        $ publicButton��5      $	��130��6'   $	 401��64   $	 138��6F   $	��160��6W   $��20(�6c   $��SemiBold"�6q   
���   �.��5      �� -	   5        $��
publictext��5      $
��Wrap=�6*   $ Center=�6>   $ 0Q�6U   Public Transport Planner6{   5      �� 5   
   -
   5        $ 
roadButton��5      $	��130��6%   $	 566��62   $	 138��6D   $	��160��6U   $��20(�6a   $��SemiBold"�6o   
���   �.��5      �� -   5        $��roadtext��5      $
��Wrap=�6(   $ Center=�6<   $ 0Q�6S   Road Planner6m   5      5      5      