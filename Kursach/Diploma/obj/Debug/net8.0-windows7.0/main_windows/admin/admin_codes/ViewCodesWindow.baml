   M S B A M L   `   `   `  ����  Diploma*    $Kursach.main_windows.ViewCodesWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$$  Мои коды доступа��5   	   $	��262��6"   $	��600��6/   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      � $��	0,0,0,131q�6   .+�5   
   �� $%��Мои коды доступа:��6   $��24(�5	      $
��Bold"�6"   $��Center=�5
      $��0,20,0,0q�61   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ CodesDataGrid��5         AutoGenerateColumns$ False��6*   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6'   $��20,60,20,-106q�5         
IsReadOnly$
 True��6*      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Роль��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6L   	   
IsReadOnly$
	 True��6V   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 63   �� *Role+6V     5      $ Код��6%   $ * 6K   $
	 True��6U   
 62   �� *Code+6U     5      $ Дата создания��6%   $ * 6Z   $
	 True��6d   
 6<   �� *	CreatedAt+6d      	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5      ��  �� Property � �� �� Value$ 0q�68   �� 5       � ��$ 10q�60   �� 5       � ��$ 14(�61   �� 5       � ��$ Stretch=�6<   �� 5       � ��$
 Left=�6<   �� 5       � ��$ 10,0,0,0q�6/   5         ColumnHeaderStyle 5!      �� 5"      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5#      ��  � �� �	#FF4D4D4D63   �� 5$       � ��
 �����63   �� 5%       � ��$
 Bold"�63   �� 5&       � ��$ 16(�61   �� 5'       � ��$ 10q�60   �� 5(       � ��$ 0q�68   �� 5)       � ��$ Stretch=�6<   5*         RowStyle 5-      �� 5.      )  #System.Windows.Controls.DataGridRow"  6   .�5/      ��  � �� �	#FF3C3C3C63   �� 50       � ��
 �����63   �� 51       � ��$ 14(�61   �� 52       � ��$ 0q�68   53      55      56      57      