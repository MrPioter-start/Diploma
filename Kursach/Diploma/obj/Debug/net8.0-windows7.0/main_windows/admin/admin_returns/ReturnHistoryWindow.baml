   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Kursach.main_windows.adminKursach.main_windows.admin  5   	   4    .Kursach.main_windows.admin.ReturnHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   :local(clr-namespace:Kursach.main_windows.admin     5   	     � Title$'  !История возвратов��5   	   $	��600��6#   $	��800��60   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $'��!История возвратов��6   $��24(�5      $
��Bold"�6"   
�������$��Center=�5      $��Center=�61   $��	0,20,0,10q�5      $��0��6'   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ReturnsDataGrid��5         AutoGenerateColumns$ False��6,   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6'   $��
20,0,20,20q�5         CanUserAddRows$ False��6'   $��1��6>      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Товар��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$	 * 6T   
   
IsReadOnly$

 True��6^   1  +System.Windows.Controls.DataGridBoundColumn   Binding 64   �� *ProductName+6^     5      $ Количество��6%   $	 * 6^   $

 True��6h    69   �� *ReturnedQuantity+6h     5      $! Время возврата��6%   $	 * 6\   $

 True��6f    6=   �� *
ReturnTime+6f     5      $ Продавец��6%   $	 * 6Y   $

 True��6c    67   �� *AdminUsername+6c      	CellStyle 5!      �� 5"       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5#      ��  �� Property � �� �� Value$ 0q�68   �� 5$       � ��$ 10q�60   �� 5%       � ��$ 14(�61   �� 5&       � ��
 �����63   �� 5'       � �� �	#FF3C3C3C63   �� 5(       � ��$ 10,0,0,0q�6/   5)         RowStyle 5,      �� 5-      )  #System.Windows.Controls.DataGridRow"  6   .�5.      ��  � �� �	#FF3C3C3C63   �� 5/       � ��
 �����63   �� 50       � ��$ 14(�61   �� 51       � ��$ 0q�68   �� 52       � ��$ Stretch=�6C   53         ColumnHeaderStyle 56      �� 57      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�58      ��  � �� �	#FF4D4D4D63   �� 59       � ��
 �����63   �� 5:       � ��$
 Bold"�63   �� 5;       � ��$ 16(�61   �� 5<       � ��$ 10q�60   �� 5=       � ��$ 0q�68   5>      5@      5A      5B      