   M S B A M L   `   `   `  ����  Diploma/    )Kursach.main_windows.CashManagementWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$'  !Управление кассой��5   	   $	��440��6#   $	��450��60   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $'��!Управление кассой��6   $��20(�6-   $
��Bold"�6;   $��Center=�5      $��	0,20,0,30q�61   $��0��6D   �� 5   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   $��0,0,0,10q�6K   $��1��6]   .+�5      �� $��Сумма:��6   $��16(�6&   $��Center=�64   $��0,0,10,0q�6O   $��77��6a   �� 5      -   5         ��Name$ AmountTextBox��5      ���	#FF4D4D4D
����������	#FF555555$��1q�5   .   $��5q�6B   $	��200��5      $��35��6"   $��14(�6.   5      �� 5   
   $ 
Horizontal=�6   $��Center=�6.   $��0,0,0,10q�6K   $��2��6]   .+�5      �� $��Операция:��6   $��16(�6)   $��Center=�67   $��0,0,10,0q�6R   �� 5      -   5         ��Name$ OperationTypeComboBox��5      ���	#FF4D4D4D
���   ����	#FF555555$��1q�5   /   $��5q�6C   $	��200��5      $��35��6#   $��14(�6/   .@�5      �� $��Пополнение��6    �� 
IsSelected$
 True��64   �� 5       $��Снятие��6   5!      5"      �� 5$   
   -   5        $��Выполнить��5$      ���	#FF00A651
�������$��0q�5&      $
��10,5q�6%   $��14(�64   $	��200��5'      $��40��6   $��Center=�6)   $��	0,10,0,20q�6F   $��3��6Y   �� 5)   
   $ Vertical=�6   $��Center=�6,   $
��0,20q�6I   $��4��6W   .+�5*      �� -   5        $%��История операций��5*      ���	#FF4D4D4D
�������$��0q�5,      $
��10,5q�6)   $��14(�68   $	��200��5-      $��40��6!   $��0,0,0,10q�6-   �� 5/      -   5        $'��!История возвратов��5/      ���	#FF3C3C3C
�������$��0q�51      $
��10,5q�6)   $��14(�68   $	��200��52      $��40��6!   53      54      55      