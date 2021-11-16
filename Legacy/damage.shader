shader_type canvas_item;
render_mode unshaded;

uniform float damage = 0.0;

float get_mult(int i)
{
	//godot shader aint got arrays. sad
	switch(i)
	{
		case 0:
			return 5.10;
		case 1:
			return 2.04;
		case 2:
			return 3.06;
		case 3:
			return 1.28;
		case 4:
			return 1.02;
		case 5:
			return 1.32;
	}
}

void modify(out vec3 col, float dam, int i)
{
	switch(i)
	{
		case 0:
			col.b -= get_mult(i) * (dam > 50.0?50.0:dam);
			break;
		case 1:
		case 2:
			col.g -= get_mult(i) * (dam > 50.0?50.0:dam);
			break;
		case 3:
		case 4:
		case 5:
			col.r -= get_mult(i) * (dam > 50.0?50.0:dam);
			break;
	}
}

vec3 rgb2hsv(vec3 c){
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz),
                vec4(c.gb, K.xy),
                step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r),
                vec4(c.r, p.yzx),
                step(p.x, c.r));
    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)),
                d / (q.x + e),
                q.x);
}

vec3 hsv2rgb(vec3 c){
    vec3 rgb = clamp(abs(mod(c.x*6.0+vec3(0.0,4.0,2.0),
                    6.0)-3.0)-1.0,
                    0.0,
                    1.0 );
    rgb = rgb*rgb*(3.0-2.0*rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

vec3 damage2color(float dam)
{
	vec3 res = vec3(255.0, 255.0, 255.0);
	
    for(int i = 0; i < 6; ++i)
	{
		modify(res, dam, i);
		dam -= 50.0;
		if(dam <= 0.0) break;
	}
	
	return res/255.0;
}

//vec4 hue_shift(vec4 color, float amount)
//{
//	vec3 hsv = rgb2hsv(color.rgb);
//	hsv.x = mod(hsv.x + amount, 1.0);
//	return vec4(hsv2rgb(hsv), color.a);
//}

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	vec3 c = damage2color(damage);
	COLOR = vec4(c, COLOR.a);
}