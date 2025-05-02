/* solution.c -- aoc 2016 24 -- troy brumley */

#include <assert.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>

#include "solution.h"

#define TXBMISC_IMPLEMENTATION
#include "txbmisc.h"

/*

###########
#0.1.....2#
#.#######.#
#4.......3#
###########

To reach all of the points of interest as quickly as possible, you
would have the robot take the following path:

    0 to 4 (2 steps)
    4 to 1 (4 steps; it can't move diagonally)
    1 to 2 (6 steps)
    2 to 3 (2 steps)

*/

/*
 * live maze is read from a file.
 */

char **maze;

/*
 * test maze from problem on website.
 */

const char *test_maze[] = {
	"###########\n",
	"#0.1.....2#\n",
	"#.#######.#\n",
	"#4.......3#\n",
	"###########\n",
	NULL
};

point
make_point(
	int n,
	int row,
	int col
) {
	return (point) {n, row, col};
}

int
find_points_in_maze(
	const char *maze[],
	point points[],
	int points_max
) {
	for (int i = 0; i < points_max; i++)
		points[i] = (point) {-1, -1, -1};
	int found = 0;
	int row = 0;
	while (maze[row] != NULL) {
		int col = 0;
		while (maze[row][col] > '\n') {
			if (is_digit(maze[row][col])) {
				found += 1;
				int n = maze[row][col] - '0';
				assert(n < points_max);
				points[n] = make_point(n, row, col);
			}
			col += 1;
		}
		row += 1;
	}
	return found;
}

int
find_shortest_path_length(point points[], int from, int to) {
	return -1;
}


int
find_shortest_paths_between(
	const char *maze[],
	point points[],
	int num_points,
	int shortest_paths[][3]
) {
	int found = 0;
	for (int from = 0; from < num_points; from++) {
		for (int to = from + 1; to < num_points; to++) {
			shortest_paths[found][0] = from;
			shortest_paths[found][1] = to;
			shortest_paths[found][2] = -1;
			found += 1;
		}
	}
	return found;
}

/*
 * part one:
 */

int
part_one(
	const char *fname
) {

	FILE *ifile = fopen(fname, "r");
	if (!ifile) {
		fprintf(stderr, "error: could not open file: %s\n", fname);
		return EXIT_FAILURE;
	}

	char iline[INPUT_LINE_MAX];

	while (fgets(iline, INPUT_LINE_MAX - 1, ifile)) {
	}

	printf("part one: %d\n", 0);

	fclose(ifile);
	return EXIT_SUCCESS;
}


/*
 * part two:
 */

int
part_two(
	const char *fname
) {
	FILE *ifile;

	ifile = fopen(fname, "r");
	if (!ifile) {
		fprintf(stderr, "error: could not open file: %s\n", fname);
		return EXIT_FAILURE;
	}
	char iline[INPUT_LINE_MAX];

	while (fgets(iline, INPUT_LINE_MAX - 1, ifile)) {
	}

	printf("part two: %d\n", 0);

	fclose(ifile);
	return EXIT_SUCCESS;
}
