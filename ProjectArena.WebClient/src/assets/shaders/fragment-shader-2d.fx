attribute vec2 a_position;
attribute vec2 a_texCoord;

uniform vec2 u_positionResolution;
uniform vec2 u_textureResolution;

varying vec2 v_texCoord;

void main() {
    vec2 tranformed_position = a_position * 2.0 / u_positionResolution - 1.0;
    gl_Position = vec4(tranformed_position * vec2(1, -1), 0, 1);

    vec2 tranformed_texCoord = a_texCoord * 2.0 / u_textureResolution - 1.0;
    v_texCoord = transformed_texCoord;
}