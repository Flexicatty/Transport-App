   M S B A M L   `   `   `  ����L  HNEA A Lvl Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:NEA_A_Lvl_ProjectNEA_A_Lvl_Project  5   	   '    !NEA_A_Lvl_Project.SelectionWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:NEA_A_Lvl_Project     5   	   M IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=nullD @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	   -   5        $��selectWindow��5
   _   0  *ModernWpf.Controls.Primitives.WindowHelper    UseModernWindowStyle  .  � Title$ SelectionWindow��6	   $	��425��6!   $	��400��6.    � WindowStartupLocation$ CenterScreen=�6:   .��5      �� -   5         ��Name$	 myC��5      .+�5   
   �� -   5         ��Name$ 
titleLable��5      $��Train Timetable��6$   $��57��6>   $	��165��6J   $��Center=�6V   $��Center=�6z   $
��Bold"�6�    �� Left$	 110��6�   $��22(�6�   �� 5   
   -   5         ��Name$ timecomboBox��5       �� Text$ Arrive��6)   $��Center=�67   $��Center=�6T   $	��124��6o   $	 130��6{   
 �� Top$	 239��6�   �� 5   
   -   5        $ fromstationcB��5      $	 130��6*   $	��124��6<   $ 72��6H   �� 5   
   -   5        $ tostationcB��5      $	 130��6(   $	��124��6:   $	 109��6F   �� 5   
   -   5        $ fromstationLable��5      $��From:��6*   $��32��6:   $��50��6F   $��Right=�6Q   $��Center=�6t   $��DemiBold"�6�   $ 72��6�   $ 75��6�   �� 5   
   -   5        $ tostationLable��5      $	��To:��6(   $��31��66   $��50��6B   $��Right=�6M   $��Center=�6p   $��DemiBold"�6�   $	 110��6�   	 J� RenderTransformOrigin$	 0.5,0.5&�6�   $ 75��6�   (  "System.Windows.Controls.DatePicker  5   
   -	   5        
  Name$
 
datePicker��5      $	 130��6)      IsTodayHighlighted . |�.     DisplayDateStart$ 
2000, 1, 1X�6g      DisplayDateEnd$ 2029, 12, 31X�6�      FirstDayOfWeek$ Monday=�6�   $	 165��6�   $��DemiBold"�6�   �� 5   
   -
   5        $ deparrcomboBox��5      $	 130��6+   $	 202��6=   $	��124��6N   $��Right=�6Z   $��Center=�6}   $��DemiBold"�6�   �� 5   
   -   5         ��Name$ enterButton��5      $	��124��6&   $	 130��62   $	 303��6D   $��31��6U   $��DemiBold"�6a   $��Center=�6w   $��Center=�6�   
�������
�����@�6�   �� 5   
   -   5        $ 	viaButton��5      $	��Via��6$   $	 259��62   $ 93��6D   $��34��6T   $��26��6_   $��11(�6k   $��DemiBold"�6y   $
��Left=�6�   $��Center=�6�   
������ 6�   � 5   
   -   5         �Name$ searchProgressBar��5      $��19��61   $ 10��6=   $	 339��6N   $	��372��6_   $y�Hidden=�6k   5      5      