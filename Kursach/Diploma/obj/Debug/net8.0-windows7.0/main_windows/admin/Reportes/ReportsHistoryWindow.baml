   M S B A M L   `   `   `  ����B  >Diploma, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY1clr-namespace:Diploma.main_windows.admin.Reportes#Diploma.main_windows.admin.Reportes  5   	   >    8Diploma.main_windows.admin.Reportes.ReportsHistoryWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Clocal1clr-namespace:Diploma.main_windows.admin.Reportes     5   	     � Title$#  История отчетов��5   	   $	��500��6!   $	��600��6.   ���	#FF2D2D30
�������.��5
      �  � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $#��История отчетов��6!   $��20(�68   $
��Bold"�6F   $��20,20,20,20q�6X   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ ReportsDataGrid��5      $��1��6,      AutoGenerateColumns$ False��69   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6'      CanUserAddRows$ False��5      $
��20,0q�6*   $��Stretch=�5      $��Stretch=�61      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ Тип отчета��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6X   	   
IsReadOnly$
	 True��6b   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 69   �� *
ReportType+6b     5       $ Дата создания��6%   $ * 6�   $
	 True��6�   
 6<   �� *GeneratedDate+ �� StringFormat$ {0:dd.MM.yyyy HH:mm:ss}��6�     5!      $ Период начала��6%   $ * 6{   $
	 True��6�   
 6<   �� *PeriodStart+$ {0:dd.MM.yyyy}��6�     5"      $% Период окончания��6%   $ * 6|   $
	 True��6�   
 6?   �� *	PeriodEnd+$ {0:dd.MM.yyyy}��6�      	CellStyle 5%      �� 5&       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5'      ��  �� Property � �� �� Value$ 0q�68   �� 5(       � ��$ 10q�60   �� 5)       � ��$ 14(�61   �� 5*       � ��
 �����63   �� 5+       � �� �	#FF3C3C3C63   5,         ColumnHeaderStyle 5/      �� 50      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�51      ��  � �� �	#FF4D4D4D63   �� 52       � ��
 �����63   �� 53       � ��$
 Bold"�63   �� 54       � ��$ 
4,10,10,10q�60   �� 55       � ��$ 0q�68   56         RowStyle 59      �� 5:      )  #System.Windows.Controls.DataGridRow"  6   .�5;      ��  � �� �	#FF3C3C3C63   �� 5<       � ��
 �����63   �� 5=       � ��$ 14(�61   �� 5>       � ��$ 0q�68   5?      5A      �� 5D   
   -   5        $��2��5D      $��Закрыть��6   ���	#FF00A651
�������$��40��5F       � Cursor$
 Hand��6   $��20,20,20,20q�6+   $��Right=�6@   5G      5H      