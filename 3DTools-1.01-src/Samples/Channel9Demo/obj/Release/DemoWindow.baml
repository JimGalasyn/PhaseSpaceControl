   M S B A M L   `   `   `  ���� I  E3DTools, Version=1.0.4544.36107, Culture=neutral, PublicKeyToken=null4'clr-namespace:_3DTools;assembly=3DTools_3DTools   Channel9Demo   Channel9Demo.DemoWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     9local'clr-namespace:_3DTools;assembly=3DTools       � Title$  Channel9Demo��$	��800��$	��500�� � 	Resources    Visual1 �� Path �� ElementName �� Mode �� Orientation �� EndPoint �� 
StartPoint � Color	 � Offset
 J� RenderTransformOrigin  Visual2 ��Name ��Name ��Name �� Orientation � Maximum  
Transform1 �� Rotation �� Angle �� Axis S� OffsetX  
Transform2  	PlaneMesh k� 	Positions k� TextureCoordinates k� TriangleIndices  	Material1 u� Brush !� Visual  	Material2��%R  &	        &	 c    &	 ,    &	 e    &	 �    &	 �    7   &	 �    7  �� ���	#FF000000$��4,4,4,4q�
�������._��� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��.H��� $
��Auto��$
��Auto��$��Stretch=�$��Stretch=�.+��� $��Stretch=�$��Stretch=����� $
 Text�$ MainTextBox��$ Default=��� $��Stretch=�$��Stretch=�$
��Auto��$	��400��$ 
Horizontal=�.+��� $��Stretch=�$��	20,0,20,0q�$��Bottom=�$��10������ $ Value�$ Slider4��$ Default=����� $	 0,1&�$	 0,0&�.��� $ 	#FF000000��$	 0Q�� $ 	#FF00EAFF��$	 1Q��� $��Stretch=�$��	20,0,20,0q�$��Bottom=�$��10������ $ Value�$ Slider3��$ Default=����� $	 0,1&�$	 0,0&�.��� $ 	#FF000000��$	 0Q�� $ 	#FF0074FF��$	 1Q��� $��Stretch=�$��	20,0,20,0q�$��Bottom=�$��10������ $ Value�$ Slider2��$ Default=����� $	 0,1&�$	 0,0&�.��� $ 	#FF000000��$	 0Q�� $ 	#FFFF0000��$	 1Q��� $��Stretch=�$��	20,0,20,0q�$��Bottom=�$��10��$
 0.5,0.5&����� $ Value�$ Slider1��$ Default=����� $	 0,1&�$	 0,0&�.��� $ 	#FF000000��$	 0Q�� $ 	#FF12FF00��$	 1Q��� ���	#FF000000$��4,4,4,4q�
�������._��� $��0,0,0,0q�$��1��$
��Auto��$
��Auto��$��Stretch=�$��Stretch=�.+��� $ MainTextBox��$��Stretch=�$��Stretch=�$
��Auto��$
��Auto��$�� ���� $��Stretch=�$��Stretch=�$
��Auto��$
��Auto��.+��� $ SliderStackPanel��$��Stretch=�$��Stretch=�$
��Auto��$	��400��$ 
Horizontal=�.+��� $ Slider4��$��Stretch=�$��Stretch=�$ Vertical=�$	 400Q�$��20,0,0,0q��� $ Slider3��$��Stretch=�$��Stretch=�$ Vertical=�$	 400Q�$��40,0,0,0q��� $ Slider2��$��Stretch=�$��Stretch=�$	 400Q�$ Vertical=�$��40,0,0,0q��� $ Slider1��$��Stretch=�$��Stretch=�$	 400Q�$ Vertical=�$��40,0,0,0q�Y� .���  �� $ 30Q�$ 0 1 05�S� $ -1Q�Y� .���  �� $	 -30Q�$ 0 1 05�S� $	 1.0Q�k�  �    �    �     u�  !� 8   u�  !� 8   .���� .+��� -    ��Name$ truthCheckbox��.��Truth!   _3DTools.TrackballDecorator  "   _3DTools.Viewport3DDecorator   Content. %   _3DTools.Interactive3DDecorator     ContainsInk .    Debug �� $ truthCheckbox��$ 	IsChecked�. '� $	��800��$	��500�� '� Camera =�  � Position$ 0, 0, 61�.x�"   _3DTools.InteractiveVisual3D   g� 	Transform# [     Geometry#  [ !   Visual#! [    # [ #  [ #! [ g� " g� Content" t� # �� Color$# 	#FFFFFFFF��$ t� 	Direction$$ 0,0,-15�