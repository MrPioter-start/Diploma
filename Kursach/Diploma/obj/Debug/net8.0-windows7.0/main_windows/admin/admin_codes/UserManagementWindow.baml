   M S B A M L   `   `   `  ����  Diploma/    )Kursach.main_windows.UserManagementWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$7  1Управление пользователями��5   	   $	��450��6+   $	��800��68   ���	#FF2D2D30
������� � WindowStartupLocation$ CenterScreen=�5   	   .��5      � .+�5   
   �� $.��(Список пользователей:��6   $��24(�5	      $
��Bold"�6"   $��Center=�5
      $��0,20,0,0q�61   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ UsersDataGrid��5         AutoGenerateColumns$ False��6*   ���	#FF3C3C3C
�������$��0q�5         GridLinesVisibility$
 None=�6'   $��20,60,20,60q�5         CanUserAddRows$ False��6(      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Логин��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6Q   	   
IsReadOnly$
	 True��6[   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 64   �� *
Username+6[     5      $ Роль��6%   $ * 6P   $
	 True��6Z   
 63   �� *
RoleName+6Z   4  .System.Windows.Controls.DataGridTemplateColumn  5      $ Действия��6)   $	 100 6;      CellTemplate 5      �� 5      .R�5      �� -   5        $��Удалить��5   %   ���	#FFD4AF37
�������$��0q�5   %   $	��5,2q�69   $��14(�6G   5      5         	CellStyle 5      �� 5       �� 
TargetType*  $System.Windows.Controls.DataGridCell"  6   .�5       ��  �� Property � �� �� Value$ 0q�68   �� 5!       � ��$ 10q�60   �� 5"       � ��$ 14(�61   �� 5#       � ��$
 Left=�6<   �� 5$       � ��$ 10,0,0,0q�6/   5%         ColumnHeaderStyle 5(      �� 5)      =  7System.Windows.Controls.Primitives.DataGridColumnHeader"  6   .�5*      ��  � �� �	#FF4D4D4D63   �� 5+       � ��
 �����63   �� 5,       � ��$
 Bold"�63   �� 5-       � ��$ 16(�61   �� 5.       � ��$ 10q�60   �� 5/       � ��$ 0q�68   50         RowStyle 53      �� 54      )	  #System.Windows.Controls.DataGridRow" 	 6   .�55      ��  � �� �	#FF3C3C3C63   �� 56       � ��
 �����63   �� 57       � ��$ 14(�61   �� 58       � ��$ 0q�68   59      5;      5<      5=      