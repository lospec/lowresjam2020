/*
	Shader excercise stuffs
	features earthbound-like wavy distortion effects
*/
shader_type canvas_item;
uniform vec2 camerazoom = vec2(1.0);

void fragment ()
{		
	const float PI = 3.146666666666;
	float t = TIME * 1.0 + 0.1;
	
	vec2 screensize = TEXTURE_PIXEL_SIZE;// * camerazoom;
	vec2 earthbounduv = floor(UV / screensize) * screensize;
	
	// earthbound-like domain warp
	const float dwarpamp = 0.25;
	float dwarpfreq = (1.0 / screensize.y) * 1.0 * PI;
	earthbounduv.x += sin(earthbounduv.y * dwarpfreq + TIME) * dwarpamp;
	
	// floor that UV
	earthbounduv = floor(earthbounduv / screensize) * screensize;
	/*
	// rotozoomer
	float scale = 2.0 + sin(TIME * PI * 0.1) * 1.0;
	float angle = TIME * PI * 0.25;
	earthbounduv += 0.5;
	earthbounduv *= mat2(vec2(cos(angle), -sin(angle)), vec2(sin(angle), cos(angle))) * scale;
	earthbounduv -= 0.5;
//	earthbounduv += t; // floor(t / SCREEN_PIXEL_SIZE) * SCREEN_PIXEL_SIZE;
	*/
	vec3 final = texture(TEXTURE, earthbounduv).rgb;
	COLOR = vec4(final, 1.0);
}
