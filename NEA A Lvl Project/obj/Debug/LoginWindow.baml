   M S B A M L   `   `   `  ����L  HNEA A Lvl Project, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:NEA_A_Lvl_ProjectNEA_A_Lvl_Project  5   	   #    NEA_A_Lvl_Project.LoginWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:NEA_A_Lvl_Project     5   	   M IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=nullD @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   5   	   -   5        $��LoginWin��5
   6   0  *ModernWpf.Controls.Primitives.WindowHelper    UseModernWindowStyle  .  � Title$ LoginWindow��6	   $	��450��6   $	��800��6*    � WindowStartupLocation$ CenterScreen=�6F   .��5      �� -   5        $	��myC��5      .+�5   
   �� -   5         ��Name$ logintextBox��5      $��13��6(    �� Left$	 325��64    �� TextWrapping$
 Wrap=�6F   $��Username��6Z   
 �� Top$	 245��6j   $	��150��6{   G� 5   
   -   5         G�Name$ loginpasswordBox��5      $��32��60   $	 325��6<   $	 282��6N   $	��150��6_   $y�Hidden=�6k   �� 5   
   -   5        $ loginpasswordtextBox��5      $��32��60   $	 325��6<   $	 282��6N   $	��150��6_   $y�Visible=�6k   $��Password��6�   �� 5   
   -   5         ��Name$ loginenterbutton��5      $��31��6+   $	 480��67   $	 283��6I   $��30��6Z   
�������6w   �� 5   
   -   5        $ logincreatebutton��5      $��No login? Press here��6,   $��35��6K   $	 325��6W   $	 349��6i   $	��150��6z   
������ 
���   �
������ ��5      �� 5      	 �� Triggers	 5      P� 5      
 P� Property
 �@J�IsMouseOver P� Value . . �5      ��  �� Property � �� �� Value
 ���� 6B   5      5      5      �� 5   
   -   5        $ returnbutton��5      $��Return��6'   $��35��68   $	 325��6D   $	 349��6V   $	��150��6g   
������ 
���   �
������ $y�Hidden=�6�   ��5      �� 5      	 5      P� 5      
 �@J�IsMouseOver . . �5       ��  � ��
 ���� 6B   5!      5#      5%      �� 5&   
   -	   5        $ returnMainButton��5&      $��Return��6+   $
��Left=�6<   $��34��6W   $	��Top=�6c   $��90��6{   $	 702��6�   
���   �$	��1.5q�6�   6�   5'      5(      