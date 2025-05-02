/* solution.h -- aoc 2016 01 -- troy brumley */

#ifndef SOLUTION_H
#define SOLUTION_H

#define INPUT_LINE_MAX 4096

extern const char *test_maze[];

typedef struct point point;
struct point {
	int n;
	int row;
	int col;
};

point
make_point(
	int n,
	int row,
	int col
);

int
find_points_in_maze(
	const char *maze[],
	point points[],
	int points_max
);

int
find_shortest_paths_between(
	const char *maze[],
	point points[],
	int num_points,
	int shortest_paths[][3]
);

int
part_one(
	const char *                       /* a file name */
);

int
part_two(
	const char *                       /* a file name */
);

/* end solution.h */
#endif /* SOLUTION_H */
