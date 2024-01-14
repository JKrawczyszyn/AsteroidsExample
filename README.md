# Asteroids example
High performant prototype of Asteroids game, without using ECS.
## Gameplay
- The map is endless.
- Ship can be steered with W, A, S, D keys. Ship shoots automatically every 0.5 seconds.
- Asteroids are destroyed after a collision with other asteroids or with the bullet.
- Destroyed asteroids are re-spawned after 1 second at random position that is outside of the player view.
## Technical details
- Created in Unity 2022.3.7f1.
- Built and tested on Android(ASUS ROG 5). For 262144 cells/asteroids constant 100-120 fps (max for phone is 120).
- Architecture is based on Views, Controllers and Models. For simplicity static variables are used.
- Data is stored in arrays for better data locality.
- Configuration parameters are in "Assets/Configs/Config".
