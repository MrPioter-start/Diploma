   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Kursach.main_windows.adminKursach.main_windows.admin  5   	   2    ,Kursach.main_windows.admin.CashHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:Kursach.main_windows.admin     5   	     � Title$%  История операций��5   	   $	��600��6"   $	��800��6/   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      �  � RowDefinitions 5   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ CashHistoryDataGrid��5         AutoGenerateColumns$ False��60   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6#   $��20,20,20,20q�5      $��0��6$      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Тип операции��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6]   	   
IsReadOnly$
	 True��6g   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 6;   �� *OperationType+6g     5      $ 
Сумма��6%   $ * 6O   $
	 True��6Y   
 64   �� *Amount+6Y     5      $ 
Время��6%   $ * 6R   $
	 True��6\   
 64   �� *	Timestamp+6\     5      $ Пользователь��6%   $ * 6]   $
	 True��6g   
 6;   �� *AdminUsername+6g      RowStyle 5      �� 5       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5      ��  �� Property � �� �� Value �	#FF3C3C3C63   �� 5       � ��
 �����63   �� 5       � ��$ 14(�61   �� 5        � ��$ 1q�68   �� 5!       � ��
 ���� 64   �� 5"       � ��$ 2q�60    �� Triggers 5#      �� 5$       �� Binding 6&   �� *OperationType+ �� Value Пополнение6H   .a�5%      ��  � �� �	#FF00A6516;   �� 5&       � �� �	#FF00A6516<   5'      �� 5(       6&   �� *OperationType+ Снятие6H   .a�5)      ��  � �� �	#FFD4AF376;   �� 5*       � �� �	#FFD4AF376<   5+      �� 5,       6&   �� *OperationType+ Возврат6H   .a�5-      ��  � �� �	#FFD4AF376;   �� 5.       � �� �	#FFD4AF376<   5/      �� 50       6&   �� *OperationType+ Инициализация6H   .a�51      ��  � �� �	#FF4D4D4D6;   �� 52       � �� �	#FF4D4D4D6<   53      55         	CellStyle 58      �� 59      *  $System.Windows.Controls.DataGridCell"  6   .�5:      ��  � ��$ 0q�68   �� 5;       � ��$ 10q�60   �� 5<       � ��$ 14(�61   �� 5=       � ��
 �����63   �� 5>       � ��
 ���� 63   �� 5?       � �� 5@      �� 5A       �� 
TargetType"  6.   .R�5B   "   �� #��z��6)   #��z��5C      #��z��5D      ._�5E   &   �� 5F   #   5G      5I      5J         ColumnHeaderStyle 5M      �� 5N      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5O      ��  � �� �	#FF4D4D4D63   �� 5P       � ��
 �����63   �� 5Q       � ��$
 Bold"�63   �� 5R       � ��$ 16(�61   �� 5S       � ��$ 
4,10,10,10q�60   �� 5T       � ��$ 0q�68   5U      5W      �� 5Y   
    �� Orientation$ 
Horizontal=�6   $��Right=�6.   $��
0,20,20,20q�6J   $��1��6^   .+�5Z      �� -   5        $��Экспорт в Excel��5Z      ���	#FF00A651
�������$��0q�5\      $
��10,5q�6%   $��14(�64    � Cursor$
 Hand��6B   $��0,0,10,0q�6P   �� 5]      -   5        $��Экспорт в Word��5]      ���	#FF4D4D4D
�������$��0q�5_      $
��10,5q�6%   $��14(�64   $
 Hand��6B   5`      5a      5b      