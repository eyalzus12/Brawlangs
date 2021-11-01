shader_type canvas_item;
render_mode unshaded;

uniform vec3 color = vec3(1, 1, 1);

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if(COLOR.xyz == vec3(1, 1, 1)) COLOR = vec4(color, COLOR.a);
}