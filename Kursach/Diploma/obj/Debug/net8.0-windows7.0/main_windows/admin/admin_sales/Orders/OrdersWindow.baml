   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Diploma.main_windowsDiploma.main_windows  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   '    !Diploma.main_windows.OrdersWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4local"clr-namespace:Diploma.main_windows     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	     � Title$  Заказы��5   	   $	��600��6   $
��1200��6%   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5	      �� 5
   
      KeyOrderStatuses   �� Type" ��6(   .s�5      ��Оформлен��6#   ��5      Собран��6!   ��5      В пути��6!   ��5      Доставлен��6$   ��5      Завершен��6#   ��5      Отменен��6"   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $!��Список заказов��6   $��24(�5      $
��Bold"�6"   $
��Left=�5      $��20,10q�6/   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrdersDataGrid��5      $��1��6+      AutoGenerateColumns$ False��68   ���	#FF3C3C3C
�������$��0q�5   =      HeadersVisibility$ Column=�5      $��20,10q�6.      CanUserAddRows$ False��6=   	   CanUserDeleteRows$	 False��5       
   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$
 * 6-   $   HorizontalScrollBarVisibility$
 Auto=�5!         Columns 5"      0  *System.Windows.Controls.DataGridTextColumn  5#      ,  &System.Windows.Controls.DataGridColumn   Header$ Клиент��6%      Width$ 2* 6T      
IsReadOnly$
 True��6_   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   �� *
ClientName+6_     5$      $ Email��6%   $
 2.5* 6N   $
 True��6[    64   �� *Email+6[     5%      $ Контакты��6%   $
 2.5* 6W   $
 True��6d    67   �� *ContactInfo+6d     5&      $ Дата��6%    5'      �� *TransactionTime+ �� StringFormat$ 
dd.MM.yyyy��  5)      $ 
Сумма��6%   $
 1.5* 6T   $
 True��6a    64   �� *TotalAmount+6a   4  .System.Windows.Controls.DataGridTemplateColumn  5*      $ Статус��6)   $ 2* 69      CellTemplate 5+      �� 5,      .R�5-      �� -   5          OrderStatuses#��[ 5-   '   ���	#FF4D4D4D
���   �$��0q�52   '   $��13(�53   '   $	��4,2q�54   '   $��Center=�55   '   56      58         RowStyle 5;      �� 5<       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5=      ��  �� Property � �� �� Value �	#FF3C3C3C63   �� 5>       � ��
 �����63   �� 5?       � ��$ 14(�61   �� 5@       � ��$ 0q�68    �� Triggers 5A      �� 5B       �� Binding 6&   �� *Status+ �� Value Завершен6A   .a�5C      ��  �@J�Opacity$	 0.5Q�68   5D      �� 5E       6&   �� *Status+ Отменен6A   .a�5F      ��  �@J�Opacity$	 0.5Q�68   5G      5I         ColumnHeaderStyle 5M      �� 5N      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5O      ��  � �� �	#FF4D4D4D63   �� 5P       � ��
 �����63   �� 5Q       � ��$
 Bold"�63   �� 5R       � ��$ 14(�61   �� 5S       � ��$
 10,4q�60   �� 5T       � ��$ 0q�68   5U      5W      5X      5Y      