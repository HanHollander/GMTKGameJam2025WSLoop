shader_type canvas_item;

uniform float scrolling_speed = 1.0;

uniform vec4 rgba : source_color;

void fragment() {
	vec2 moving_uv = vec2(UV.x-(TIME*scrolling_speed), UV.y);
	vec4 in_tex = texture(TEXTURE, moving_uv);
    COLOR = in_tex * vec4(rgba);
	//COLOR = vec4(rgba);
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
