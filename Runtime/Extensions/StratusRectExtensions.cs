using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Stratus.Extensions;

namespace Stratus
{
	/// <summary>
	/// Sourced from: https://github.com/slavniyteo/rect-ex
	/// </summary>
	public static class StratusRectExtensions
	{
		private const float defaultSpacing = 2f;

		private class Cell
		{
			public float Weight { get; private set; }
			public float FixedWidth { get; private set; }

			public Cell(float weight, float fixedWidth)
			{
				this.Weight = weight;
				this.FixedWidth = fixedWidth;

			}

			public bool HasWidth { get { return FixedWidth > 0 || Weight > 0; } }
			public float GetWidth(float weightUnit)
			{
				return FixedWidth + Weight * weightUnit;
			}
		}

		#region Row
		public static Rect[] Row(this Rect rect, float[] weights, float space = defaultSpacing)
		{
			return Row(rect, weights, null, space);
		}

		public static Rect[] Row(this Rect rect, float[] weights, float[] widthes, float space = defaultSpacing)
		{
			if (weights == null)
			{
				throw new ArgumentException("Weights is null. You must specify it");
			}

			if (widthes == null)
			{
				widthes = Enumerable.Repeat(0f, weights.Length).ToArray();
			}

			rect = rect.Abs();
			return RowSafe(rect, weights, widthes, space);
		}

		public static Rect[] Row(this Rect rect, int count, float space = defaultSpacing)
		{
			rect = rect.Abs();
			switch (count)
			{
				case 1:
					{
						return new Rect[] { rect };
					}
				case 2:
					{
						return RowTwoSlices(rect, space);
					}
				case 3:
					{
						return RowThreeSlices(rect, space);
					}
				default:
					{
						var weights = Enumerable.Repeat(1f, count).ToArray();
						var widthes = Enumerable.Repeat(0f, count).ToArray();
						return Row(rect, weights, widthes, space);
					}
			}
		}

		private static Rect[] RowSafe(Rect rect, float[] weights, float[] widthes, float space)
		{
			var cells = weights.Merge(widthes, (weight, width) => new Cell(weight, width)).Where(cell => cell.HasWidth);

			float weightUnit = GetWeightUnit(rect.width, cells, space);

			var result = new List<Rect>();
			float nextX = rect.x;
			foreach (var cell in cells)
			{
				result.Add(new Rect(
							   x: nextX,
							   y: rect.y,
							   width: cell.GetWidth(weightUnit),
							   height: rect.height
						   ));

				nextX += cell.HasWidth ? (cell.GetWidth(weightUnit) + space) : 0;
			}

			return result.ToArray();
		}

		private static float GetWeightUnit(float fullWidth, IEnumerable<Cell> cells, float space)
		{
			float result = 0;
			float weightsSum = cells.Sum(cell => cell.Weight);

			if (weightsSum > 0)
			{
				float fixedWidth = cells.Sum(cell => cell.FixedWidth);
				float spacesWidth = (cells.Count(cell => cell.HasWidth) - 1) * space;
				result = (fullWidth - fixedWidth - spacesWidth) / weightsSum;
			}

			return result;
		}

		private static Rect[] RowTwoSlices(Rect rect, float space)
		{
			var first = new Rect(
				x: rect.x,
				y: rect.y,
				width: (rect.width - space) / 2,
				height: rect.height
			);
			var second = new Rect(
				x: first.x + space + first.width,
				y: first.y,
				width: first.width,
				height: first.height
			);
			return new Rect[] { first, second };
		}

		private static Rect[] RowThreeSlices(Rect rect, float space)
		{
			var first = new Rect(
				x: rect.x,
				y: rect.y,
				width: (rect.width - 2 * space) / 3,
				height: rect.height
			);
			var second = new Rect(
				x: first.x + first.width + space,
				y: rect.y,
				width: first.width,
				height: first.height
			);
			var third = new Rect(
				x: second.x + second.width + space,
				y: second.y,
				width: second.width,
				height: second.height
			);
			return new Rect[] { first, second, third };
		}
		#endregion

		#region Column
		public static Rect[] Column(this Rect rect, int count, float space = defaultSpacing)
		{
			rect = rect.Invert();
			var result = rect.Row(count, space);
			return result.Select(x => x.Invert()).ToArray();
		}

		public static Rect[] Column(this Rect rect, float[] weights, float space = defaultSpacing)
		{
			return Column(rect, weights, null, space);
		}

		public static Rect[] Column(this Rect rect, float[] weights, float[] widthes, float space = defaultSpacing)
		{
			rect = rect.Invert();
			var result = rect.Row(weights, widthes, space);
			return result.Select(x => x.Invert()).ToArray();
		}
		#endregion

		#region Grid
		public static Rect[,] Grid(this Rect rect, int rows, int columns, float space = defaultSpacing)
		{
			return Grid(rect, rows, columns, space, space);
		}

		public static Rect[,] Grid(this Rect rect, int rows, int columns, float spaceBetweenRows, float spaceBetweenColumns)
		{
			var grid = rect.Column(rows, spaceBetweenRows)
						   .Select(x => x.Row(columns, spaceBetweenColumns))
						   .ToArray();

			var result = new Rect[rows, columns];
			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < columns; column++)
				{
					result[row, column] = grid[row][column];
				}
			}
			return result;
		}

		public static Rect[,] Grid(this Rect rect, int size, float space = defaultSpacing)
		{
			return Grid(rect, size, size, space, space);
		}
		#endregion

		#region Cut
		public static Rect[] CutFromRight(this Rect rect, float width, float space = defaultSpacing)
		{
			var second = Rect.MinMaxRect(
				xmin: rect.xMax - width,
				xmax: rect.xMax,
				ymin: rect.yMin,
				ymax: rect.yMax
			);
			float min = Math.Min(rect.xMin, second.xMin - space);
			var first = Rect.MinMaxRect(
				xmin: min,
				xmax: second.xMin - space,
				ymin: rect.yMin,
				ymax: rect.yMax
			);
			return new Rect[] { first, second };
		}

		public static Rect[] CutFromBottom(this Rect rect, float height, float space = defaultSpacing)
		{
			var second = Rect.MinMaxRect(
				xmin: rect.xMin,
				xmax: rect.xMax,
				ymin: rect.yMax - height,
				ymax: rect.yMax
			);
			float min = Math.Min(rect.yMin, second.yMin - space);
			var first = Rect.MinMaxRect(
				xmin: rect.xMin,
				xmax: rect.xMax,
				ymin: min,
				ymax: second.yMin - space
			);
			return new Rect[] { first, second };
		}

		public static Rect[] CutFromLeft(this Rect rect, float width, float space = defaultSpacing)
		{
			var first = Rect.MinMaxRect(
				xmin: rect.xMin,
				xmax: rect.xMin + width,
				ymin: rect.yMin,
				ymax: rect.yMax
			);
			float max = Math.Max(rect.xMax, first.xMax + space);
			var second = Rect.MinMaxRect(
				xmin: first.xMax + space,
				xmax: max,
				ymin: rect.yMin,
				ymax: rect.yMax
			);
			return new Rect[] { first, second };
		}

		public static Rect[] CutFromTop(this Rect rect, float height, float space = defaultSpacing)
		{
			var first = Rect.MinMaxRect(
				xmin: rect.xMin,
				xmax: rect.xMax,
				ymin: rect.yMin,
				ymax: rect.yMin + height
			);
			float max = Math.Max(rect.yMax, first.yMax + space);
			var second = Rect.MinMaxRect(
				xmin: rect.xMin,
				xmax: rect.xMax,
				ymin: first.yMax + space,
				ymax: max
			);
			return new Rect[] { first, second };
		}
		#endregion

		#region Helpers
		public static Rect Abs(this Rect rect)
		{
			if (rect.width < 0)
			{
				rect.x += rect.width;
				rect.width *= -1;
			}
			if (rect.height < 0)
			{
				rect.y += rect.height;
				rect.height *= -1;
			}
			return rect;
		}

		public static Rect Invert(this Rect rect)
		{
			return new Rect(
				x: rect.y,
				y: rect.x,
				width: rect.height,
				height: rect.width
			);
		}

		public static Rect Union(this Rect rect, params Rect[] other)
		{
			if (other == null || other.Length == 0)
			{
				return rect;
			}
			else if (other.Length == 1 && other[0] == rect)
			{
				return rect;
			}
			else
			{
				var xMin = Math.Min(rect.xMin, other.Select(x => x.xMin).Aggregate(Math.Min));
				var yMin = Math.Min(rect.yMin, other.Select(x => x.yMin).Aggregate(Math.Min));
				var xMax = Math.Max(rect.xMax, other.Select(x => x.xMax).Aggregate(Math.Max));
				var yMax = Math.Max(rect.yMax, other.Select(x => x.yMax).Aggregate(Math.Max));
				return Rect.MinMaxRect(
					xmin: xMin,
					xmax: xMax,
					ymin: yMin,
					ymax: yMax
				);
			}
		}

		public static Rect Intend(this Rect rect, float border)
		{
			rect = rect.Abs();

			var result = new Rect(
				x: rect.x + border,
				y: rect.y + border,
				width: rect.width - 2 * border,
				height: rect.height - 2 * border
			);

			if (result.width < 0)
			{
				result.x += result.width / 2;
				result.width = 0;
			}
			if (result.height < 0)
			{
				result.y += result.height / 2;
				result.height = 0;
			}
			return result;
		}

		public static Rect Extend(this Rect rect, float border)
		{
			rect = rect.Abs();
			return new Rect(
				x: rect.x - border,
				y: rect.y - border,
				width: rect.width + 2 * border,
				height: rect.height + 2 * border
			);
		}
		#endregion
	}

}