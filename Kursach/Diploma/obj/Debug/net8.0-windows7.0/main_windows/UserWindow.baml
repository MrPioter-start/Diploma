   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:Kursach.main_windowsKursach.main_windows  5   	   %    Kursach.main_windows.UserWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   4local"clr-namespace:Kursach.main_windows     5   	     � Title$  Пользователь��5   
   $	��600��6   $	��800��6,   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      � .+�5   
   ]� ���	#FF3C3C3C$��60��6+   $	��Top=�67   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�62   $
��20,0q�6M   $��Right=�6[   .+�5      �� $��Поиск: ��6   
�������$��18(�6=   �� 5      -   5         ��Name$ SearchTextBox��5      ���	#FF4D4D4D
����������	#FF555555$��1q�5   .   $��10q�6B   $��40��6O   $	��200��5      $��0,0,10,0q�6#   5      5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ProductsDataGrid��5         AutoGenerateColumns$ False��6-   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6   $��20,80,20,20q�5         CanUserAddRows$ False��6       Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn	   Header$	 Название��6%   
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 * 6P      
IsReadOnly$
 True��6Z   1  +System.Windows.Controls.DataGridBoundColumn   Binding 67   �� *Name+6Z     5      $	 Категория��6%   $
 * 6Y   $
 True��6c    68   �� *CategoryName+6c     5      $	 Цена��6%   $
 * 6M   $
 True��6W    63   �� *Price+6W     5      $	 Количество��6%   $
 * 6V   $
 True��6`    69   �� *
Quantity+6`      	CellStyle 5       �� 5!       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5"      ��  �� Property � �� �� Value$ 0q�68   �� 5#       � ��$ 10q�60   �� 5$       � ��$ 14(�61   �� 5%       � ��$
 Left=�6<   5&         ColumnHeaderStyle 5)      �� 5*      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5+      ��  � �� �	#FF4D4D4D63   �� 5,       � ��
 �����63   �� 5-       � ��$
 Bold"�63   �� 5.       � ��$ 16(�61   �� 5/       � ��$ 10q�60   �� 50       � ��$ 0q�68   51         RowStyle 54      �� 55      )  #System.Windows.Controls.DataGridRow"  6   .�56      ��  � �� �	#FF3C3C3C63   �� 57       � ��
 �����63   �� 58       � ��$ 14(�61   �� 59       � ��$ 0q�68   �� 5:       � ��$
 Left=�6<   5;      5=      5>      5?      