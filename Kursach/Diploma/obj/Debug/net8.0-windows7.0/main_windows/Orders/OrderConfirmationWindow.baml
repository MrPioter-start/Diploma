   M S B A M L   `   `   `  ����  Diploma2    ,Diploma.main_windows.OrderConfirmationWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$'  !Оформление заказа��5   	   $	��600��6#   $	��800��60   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   ���	#FF4D4D4D �� CornerRadius$ 5��65   $��20,10q�6F    �� Padding$ 15q�6U   ._�5      ��  �� Orientation$ Vertical=�6   .+�5      �� $-��'Информация о клиенте:��6   $
��Bold"�69   $��16(�6K   $��0,0,0,10q�6Y   �� 5      $��14(�68   $��0,0,0,5q�6F   ��6   �� *
ClientName+6F   �� 5      $��14(�69   $��0,0,0,5q�6G   ��6   �� *ContactInfo+6G   �� 5      $��14(�63   $��0,0,0,5q�6A   ��6   �� *Email+6A   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderProductsDataGrid��5      $��1��62      AutoGenerateColumns$ False��6?   ���	#FF3C3C3C
�������$��0q�5   5   $��20,10q�5         CanUserAddRows$ False��6   	   CanUserDeleteRows$	 False��61   
   Columns
 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$+ %Наименование товара��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6[      
IsReadOnly$
 True��6e   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6B   �� *Name+6e     5      $ 
Бренд��6%   $ * 6N   $
 True��6X    64   �� *Brand+6X     5      $ Количество��6%   $ * 6[   $
 True��6e    69   �� *OrderQuantity+6e     5       $ Цена��6%   $ * 6M   $
 True��6W    63   �� *Price+6W      RowStyle 5$      �� 5%       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5&      ��  �� Property � �� �� Value �	#FF3C3C3C63   �� 5'       � ��
 �����63   �� 5(       � ��$ 14(�61   �� 5)       � ��$ 0q�68   5*         ColumnHeaderStyle 5.      �� 5/      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�50      ��  � �� �	#FF4D4D4D63   �� 51       � ��
 �����63   �� 52       � ��$
 Bold"�63   �� 53       � ��$ 14(�61   �� 54       � ��$ 10q�60   �� 55       � ��$ 0q�68   56      58      �� 5;   
   -   5        $'��!Подтвердить заказ��5;      $��2��6-   ���	#FF00A651
�������$��0q�5=      $��15,10q�6%   $��14(�65    � Cursor$
 Hand��6C   $��Right=�5>      $��
0,10,20,10q�6-   $	��200��6A   5?      5@      