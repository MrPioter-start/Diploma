   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Diploma.main_windowsDiploma.main_windows  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	   '    !Diploma.main_windows.OrdersWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   4local"clr-namespace:Diploma.main_windows     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	     � Title$  Заказы��5   	   $	��600��6   $
��1000��6%   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5	      �� 5
   
      KeyOrderStatuses   �� Type" ��6(   .s�5      ��Оформлен��6#   ��5      Собран��6!   ��5      В пути��6!   ��5      Доставлен��6$   ��5      Завершен��6#   ��5      Отменен��6"   5      .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $!��Список заказов��6   $��24(�5      $
��Bold"�6"   $
��Left=�5      $��20,10q�6/   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrdersDataGrid��5      $��1��6+      AutoGenerateColumns$ False��68   ���	#FF3C3C3C
�������$��0q�5   =      HeadersVisibility$ Column=�5      $��20,10q�6.      CanUserAddRows$ False��6=   	   CanUserDeleteRows$	 False��6T   
   Columns
 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ Клиент��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6T      
IsReadOnly$
 True��6^   1  +System.Windows.Controls.DataGridBoundColumn   Binding 65   �� *
ClientName+6^     5"      $ Email��6%   $ * 6N   $
 True��6X    64   �� *Email+6X     5#      $/ )Контактная информация��6%   $ * 6d   $
 True��6n    6D   �� *ContactInfo+6n     5$      $ 
Сумма��6%   $ * 6T   $
 True��6^    64   �� *TotalAmount+6^   4  .System.Windows.Controls.DataGridTemplateColumn  5%      $ Статус��6)   $ * 69      CellTemplate 5&      �� 5'      .R�5(      �� -   5          OrderStatuses#��[ 5)      5+      5,      5.         RowStyle 51      �� 52       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�53      ��  �� Property � �� �� Value �	#FF3C3C3C63   �� 54       � ��
 �����63   �� 55       � ��$ 14(�61   �� 56       � ��$ 0q�68   57         ColumnHeaderStyle 5:      �� 5;      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5<      ��  � �� �	#FF4D4D4D63   �� 5=       � ��
 �����63   �� 5>       � ��$
 Bold"�63   �� 5?       � ��$ 14(�61   �� 5@       � ��$ 10q�60   �� 5A       � ��$ 0q�68   5B      5D      5E      5F      