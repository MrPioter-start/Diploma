   M S B A M L   `   `   `  ����  Diploma9    3Diploma.main_windows.admin.TransactionDetailsWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$'  !Детали транзакции��5   	   $	��500��6#   $	��800��60   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      �  � RowDefinitions 5	   
   �� 5
      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� ���	#FF3C3C3C �� Padding$ 20q�6(   $��0��65   ._�5      �� $'��!Детали транзакции��6   $��24(�5      $
��Bold"�6&   
�������$��Center=�5      $��Center=�65   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$  TransactionDetailsDataGrid��5         AutoGenerateColumns$ False��67   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6'   $��
20,0,20,20q�5         CanUserAddRows$ False��6'   $��1��6>      
IsReadOnly$
 True��6K   	   Columns	 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn
   Header$
 ID продажи��6%      
Visibility$ Hidden=�6T   1  +System.Windows.Controls.DataGridBoundColumn   Binding 69   �� *SaleID+6T     5       $
 
Товар��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6T      
IsReadOnly$
 True��6^    64   �� *ProductName+6^     5!      $
 Количество��6%   $ * 6V   $
 True��6`    69   �� *
Quantity+6`     5"      $
 Цена��6%   $ * 6h   $
 True��6r    63   �� *Price+ �� StringFormat$ 
{0:F2} byn��6r      	CellStyle 5&      �� 5'       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5(      ��  �� Property � �� �� Value$ 0q�68   �� 5)       � ��$ 10q�60   �� 5*       � ��$ 14(�61   �� 5+       � ��$
 Left=�6<   �� 5,       � |�$ False��62   �� 5-       � ��$ False��62   5.         ColumnHeaderStyle 52      �� 53      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�54      ��  � �� �	#FF4D4D4D63   �� 55       � ��
 �����63   �� 56       � ��$
 Bold"�63   �� 57       � ��$ 14(�61   �� 58       � ��$ 
3,10,10,10q�60   �� 59       � ��$ 0q�68   5:         RowStyle 5>      �� 5?      )  #System.Windows.Controls.DataGridRow"  6   .�5@      ��  � �� �	#FF3C3C3C63   �� 5A       � ��
 �����63   �� 5B       � ��$ 14(�61   �� 5C       � ��$ 0q�68   �� 5D       � ��$ 40��6/   5E      5G      �� 5J   
    �� Orientation$ 
Horizontal=�6   $��Right=�6.   $��
0,10,20,10q�6J   $��2��6^   .+�5K      �� -   5         ��Name$ ReturnButton��5K      $��Возврат��6+   ���	#FFD4AF37
�������$��40��5M      $	��150��6!   $
��10,5q�6-    � Cursor$
 Hand��6<   $��0q�5N      $
��Bold"�6)   $��14(�6;   5O      5P      5Q      