   M S B A M L   `   `   `  ����  Diploma3    -Diploma.main_windows.OrderConfirmationWindow2   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$-  'Подтверждение заказа��5   	   $	��600��6&   $	��800��63   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��0,0,0,20q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��Детали заказа��6   $��26(�5      $
��Bold"�6"   $��Center=�5      $��	0,20,0,16q�61   ���	#FF00A6515      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderDetailsDataGrid��5         AutoGenerateColumns$ False��61   ���	#FF3C3C3C
�������$��0q�5   =   $��	20,0,20,0q�5         CanUserAddRows$ False��6&   $��1��6=      HeadersVisibility$ Column=�5         
IsReadOnly$
 True��5         SelectionMode$ Single=�5      	   SelectionUnit$	 FullRow=�5      
   AlternatingRowBackground
 �	#FF4D4D4D   RowBackground �	#FF3C3C3C   GridLinesVisibility$
 None=�5         Columns 5       0  *System.Windows.Controls.DataGridTextColumn  5!      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6O      
Visibility$ Hidden=�6Z   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *	ProductID+6Z     5"      $ 
Товар��6%   $ * 6T    64   �� *ProductName+6T     5#      $ Количество��6%   $ * 6[    69   �� *OrderQuantity+6[     5$      $" Цена за единицу��6%   $ * 6X    6>   �� *Price+6X     5%      $ 
Сумма��6%   $ * 6N    64   �� *Total+6N      RowStyle 5)      �� 5*       �� 
TargetType)  #System.Windows.Controls.DataGridRow"  6   .�5+      ��  �� Property � �� �� Value$ 14(�61   �� 5,       � ��$ 32��62   �� 5-       � ��
 �����63   �� 5.       � ��
 ���� 63    �� Triggers 5/      P� 50       P� Property �@ 
IsSelected P� Value$
 True��68   . �51      ��  � �� �	#FF2A2A2A6;   �� 52       � ��
 �����6;   53      P� 54       �@��AlternationIndex$ 1��6K   . �55      ��  � �� �	#FF4D4D4D6;   56      58         	CellStyle 5<      �� 5=      *  $System.Windows.Controls.DataGridCell"  6   .�5>      ��  � ��$	 8,4q�60   �� 5?       � ��$ 0q�68   �� 5@       � ��
 ���� 64   �� 5A       � ��
 ���� 63   �� 5B       � ��
 �����63   �� 5C       � ��$ 14(�61   5D         ColumnHeaderStyle 5H      �� 5I      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5J      ��  � �� �	#FF4D4D4D63   �� 5K       � ��
 �����63   �� 5L       � ��$
 Bold"�63   �� 5M       � ��$ 15(�61   �� 5N       � ��$
 4,10q�60   �� 5O       � ��$ 0q�68   �� 5P       � ��$
 Left=�6C   5Q      5S      �� 5V   
    �� Orientation$ Vertical=�6   $��2��6,   $��
20,16,20,0q�69   .+�5W      �� -   5         ��Name$ TotalPriceTextBlock��5W      ���	#FF00A651$��18(�5Y      $
��Bold"�6&   $��Right=�5Z      $��0,0,0,10q�64   �� 5\      $ 
Horizontal=�6   $��Right=�62   .+�5]      �� -   5        $��Отменить��5]      ���	#FFAF3D3D
�������$��0q�5_      $
��12,6q�6-   $��14(�5`       � Cursor$
 Hand��6'   $��0,0,10,0q�65   
������ $	��120��5b      $��36��6%   $��Right=�5c       c� ButtonStyleKey#���  5d      �� 5f      -   5        $��Подтвердить��5f      ���	#FF00A651
�������$��0q�5h      $
��12,6q�6-   $��14(�5i      $
 Hand��6'   $	��120��5j      $��36��6%   
������ $��Right=�5l      #���  5m      5n      5o      5p      5q      