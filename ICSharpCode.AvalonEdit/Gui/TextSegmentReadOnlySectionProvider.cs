// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Gui
{
	/// <summary>
	/// Implementation for <see cref="IReadOnlySectionProvider"/> that stores the segments
	/// in a <see cref="TextSegmentCollection{T}"/>.
	/// </summary>
	public class TextSegmentReadOnlySectionProvider<T> : IReadOnlySectionProvider where T : TextSegment
	{
		readonly TextSegmentCollection<T> segments;
		
		/// <summary>
		/// Gets the collection storing the read-only segments.
		/// </summary>
		public TextSegmentCollection<T> Segments {
			get { return segments; }
		}
		
		/// <summary>
		/// Creates a new TextSegmentReadOnlySectionProvider instance for the specified document.
		/// </summary>
		public TextSegmentReadOnlySectionProvider(TextDocument textDocument)
		{
			segments = new TextSegmentCollection<T>(textDocument);
		}
		
		/// <summary>
		/// Creates a new TextSegmentReadOnlySectionProvider instance for the specified document.
		/// </summary>
		public TextSegmentReadOnlySectionProvider(TextSegmentCollection<T> segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");
			this.segments = segments;
		}
		
		/// <summary>
		/// Gets whether insertion is possible at the specified offset.
		/// </summary>
		public virtual bool CanInsert(int offset)
		{
			foreach (TextSegment segment in segments.FindSegmentsContaining(offset)) {
				if (segment.StartOffset < offset && offset < segment.EndOffset)
					return false;
			}
			return true;
		}
		
		/// <summary>
		/// Gets the deletable segments inside the given segment.
		/// </summary>
		public virtual IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");
			
			int readonlyUntil = segment.Offset;
			foreach (TextSegment ts in segments.FindOverlappingSegments(segment)) {
				int start = ts.StartOffset;
				int end = start + ts.Length;
				if (start > readonlyUntil) {
					yield return new SimpleSegment(readonlyUntil, start - readonlyUntil);
				}
				if (end > readonlyUntil) {
					readonlyUntil = end;
				}
			}
			int endOffset = segment.EndOffset;
			if (readonlyUntil < endOffset) {
				yield return new SimpleSegment(readonlyUntil, endOffset - readonlyUntil);
			}
		}
	}
}
